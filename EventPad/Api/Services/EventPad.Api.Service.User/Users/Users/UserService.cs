﻿
using AutoMapper;
using EventPad.Actions;
using EventPad.Api.Context.Entities;
using EventPad.Common;
using EventPad.Redis;
using EventPad.Services.Actions;
using EventPad.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace EventPad.Api.Service.Users;

public class UserService : IUserService
{
    private readonly ILogger logger;
    private readonly IMapper mapper;
    private readonly UserManager<User> userManager;
    private readonly IRightsService rightsService;
    private readonly IModelValidator<RegiserUserModel> registerUserModelValidator;
    private readonly IAction action;
    private readonly IRedisService redisService;
    private readonly MainSettings mainSettings;

    public UserService(
        IMapper mapper,
        UserManager<User> userManager,
        IModelValidator<RegiserUserModel> registerUserModelValidator,
        IAction action,
        ILogger logger,
        IRightsService rightsService,
        IRedisService redisService,
        MainSettings mainSettings)
    {
        this.mapper = mapper;
        this.userManager = userManager;
        this.registerUserModelValidator = registerUserModelValidator;
        this.action = action;
        this.logger = logger;
        this.rightsService = rightsService;
        this.redisService = redisService;
        this.mainSettings = mainSettings;
    }

    public async Task<bool> IsEmpty()
    {
        return !await userManager.Users.AnyAsync();
    }

    public async Task<IEnumerable<UserModel>> GetAllUsers(int page = 1, int pageSize = 10, UserModelFilter filter = null)
    {
        var users = await userManager.Users
            .ToListAsync();

        return mapper.Map<IEnumerable<UserModel>>(users);
    }

    public async Task<UserModel> GetById(Guid id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user == null)
            throw new ProcessException($"User {id} not found");

        return mapper.Map<UserModel>(user);
    }

    public async Task<UserModel> Create(RegiserUserModel model)
    {
        registerUserModelValidator.Check(model);

        var user = await userManager.FindByEmailAsync(model.Email);
        if (user != null)
            throw new ProcessException($"User account with email {model.Email} already exist.");

        var avatar = "avatar.png";

        user = new User()
        {
            FirstName = model.FirstName,
            SecondName = model.SecondName,
            Role = UserRole.Regular,
            Rating = 0,
            Image = avatar,

            UserName = model.Email,
            Email = model.Email,
            EmailConfirmed = true,
            PhoneNumber = null,
            PhoneNumberConfirmed = false
        };

        var result = await userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            throw new ProcessException($"Creating user account is wrong. {string.Join(", ", result.Errors.Select(s => s.Description))}");

        user.Account = user.Id;

        result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new ProcessException($"Creating user account is wrong. {string.Join(", ", result.Errors.Select(s => s.Description))}");

        await action.CreateUserAccount(new CreateUserAccount()
        {
            Id = user.Id,
        });

        await action.SendEmail(new SendEmailModel()
        {
            Email = model.Email,
            Subject = "EventPad message",
            Message = $"Thank you, {user.FirstName}, for registering on our portal.\n\rBest regards, EventPad administration."
        });

        return mapper.Map<UserModel>(user);
    }

    public async Task<UserModel> Update(Guid id, UpdateUserModel model, Guid userId)
    {
        if (!await rightsService.IsAdmin(userId))
        {
            if (userId != id)
                throw new ProcessException($"You don't have access to this feature");
        }

        var user = await userManager.FindByIdAsync(id.ToString());
        if (user == null)
            throw new ProcessException($"User {id} not found");

        var oldImage = user.Image;
        user = mapper.Map(model, user);

        if (model.Image != null)
        {
            var fileName = await model.Image.SaveToFile(Path.Combine(mainSettings.RootDir, mainSettings.FileDir));

            user.Image = fileName;
        }
        else
        {
            user.Image = oldImage;
        }

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new ProcessException($"Updating user account is wrong. {string.Join(", ", result.Errors.Select(s => s.Description))}");

        return mapper.Map<UserModel>(user);
    }

    public async Task Delete(Guid id, Guid userId)
    {
        if (!await rightsService.IsAdmin(userId))
        {
            if (userId != id)
                throw new ProcessException($"You don't have access to this feature");
        }

        var user = await userManager.FindByIdAsync(id.ToString());
        if (user == null)
            throw new ProcessException($"User {id} not found");

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
            throw new ProcessException($"Deleting user account is wrong. {string.Join(", ", result.Errors.Select(s => s.Description))}");
    }

    public async Task SetRights(Guid id, UserRole role, Guid userId)
    {
        if (!await rightsService.IsAdmin(userId))
            throw new ProcessException($"You don't have access to this feature");

        rightsService.SetRights(id, role);
    }
}
