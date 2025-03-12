﻿global using AutoMapper;
global using FluentValidation;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Logging;
global using OnlineExam.Core.Dtos.Pagination;
global using OnlineExam.Core.ViewModels.Question.TrueOrFalse;
global using OnlineExam.Core.IRepositories.Generic;
global using OnlineExam.Core.IServices;
global using OnlineExam.Core.IUnit;
global using OnlineExam.Core.ViewModels.Exam.Request;
global using OnlineExam.Core.ViewModels.Exam.Response;
global using OnlineExam.Domain.Entities;
global using OnlineExam.Domain.Entities.Identity;
global using OnlineExam.Shared.Exceptions.Base;
global using Microsoft.AspNetCore.Identity;
global using OnlineExam.Core.IServices.Provider;
global using OnlineExam.Core.ViewModels.Auth.Requests;
global using OnlineExam.Core.ViewModels.Auth.Responses;
global using OnlineExam.Core.ViewModels.Question.choose.Requests;
global using System.Linq.Expressions;