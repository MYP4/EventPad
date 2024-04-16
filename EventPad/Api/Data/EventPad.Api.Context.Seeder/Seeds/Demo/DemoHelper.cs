﻿namespace EventPad.Api.Context.Seeder;

using EventPad.Api.Context.Entities;
using Microsoft.AspNetCore.Identity;

public class DemoHelper
{
    private readonly UserManager<User> userManager;

    public DemoHelper(UserManager<User> userManager)
    {
        this.userManager = userManager;
    }

    public async Task<IEnumerable<Event>> GetEvents()
    {
        var userId1 = Guid.Parse("9766f76a-fdb0-4b21-bd03-625710a3f1f9");
        var user1 = new User()
        {
            Id = userId1,
            FirstName = "Василий",
            SecondName = "Петров",
            Role = UserRole.Regular,
            Rating = 3,
            Account = userId1,

            UserName = "Petrov@pad.com",
            EmailConfirmed = true,
            PhoneNumber = null,
            PhoneNumberConfirmed = false,
        };
        await userManager.CreateAsync(user1);
        await userManager.AddPasswordAsync(user1, "qwertyui");


        var userId2 = Guid.Parse("29252196-f44e-49e0-b69f-8a08ec21d27b");
        var user2 = new User()
        {
            Id = userId2,
            FirstName = "Ольга",
            SecondName = "Анисимова",
            Role = UserRole.Regular,
            Rating = 3,
            Account = userId2,

            UserName = "a.olga@pad.com",
            EmailConfirmed = true,
            PhoneNumber = null,
            PhoneNumberConfirmed = false,
        };
        await userManager.CreateAsync(user2);
        await userManager.AddPasswordAsync(user2, "12345678");

        var userId3 = Guid.Parse("43ba55b1-d0e9-44fc-aa7a-55263e8c721d");
        var user3 = new User()
        {
            Id = userId3,
            FirstName = "Максим",
            SecondName = "Гусев",
            Role = UserRole.Regular,
            Rating = 3,
            Account = userId3,

            UserName = "GusevMaks@pad.com",
            EmailConfirmed = true,
            PhoneNumber = null,
            PhoneNumberConfirmed = false,
        };
        await userManager.CreateAsync(user3);
        await userManager.AddPasswordAsync(user3, "MaksGus");


        return new List<Event>
        {
            new Event()
            {
                Id = 1,
                Uid = Guid.NewGuid(),
                Name = "Волейбол",
                Description = "1 час",
                Price = 150,
                Address = "ВГУ",
                Type = EventType.Multiple,
                Image = "40ca53b3914545a1bea0e700a40de577_volleyball.jpg",
                Admin = user1
            },
            new Event()
            {
                Id = 2,
                Uid = Guid.NewGuid(),
                Name = "Баскетбол",
                Description = "1 час",
                Price = 100,
                Address = "ВГУ",
                Type = EventType.Single,
                Image = "6d9dd28f491f4d0081a5a5d3da828d18_basketball.jpg",
                Admin = user2
            },
            new Event()
            {
                Id = 3,
                Uid = Guid.NewGuid(),
                Name = "Футбол",
                Description = "2 часа",
                Price = 200,
                Address = "ВГУ",
                Type = EventType.Single,
                Image = "7b5d5f54e04f4f51ae597a8ecd9526d1_football.jpg",
                Admin = user2
            }
        };
    }
}
