﻿using AutoMapper;
using EventPad.Api.Context.Entities;
using EventPad.Api.Service.Users;

namespace EventPad.Api.Controllers.Users;

public class UserResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string SecondName { get; set; }
    public UserRole Role { get; set; }
    public float Rating { get; set; }
    public string Email { get; set; }
    public string Image { get; set; }
}


public class UserResponceProfile : Profile
{
    public UserResponceProfile()
    {
        CreateMap<UserModel, UserResponse>();
        CreateMap<UserProfileModel, UserResponse>()
            .ForMember(dest => dest.Role, opt => opt.Ignore());
    }
}