﻿global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using OnlineExam.Core.AutoMapper;
global using OnlineExam.Infrastructure.Data.context;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.RateLimiting;
global using OnlineExam.Business.Services;
global using OnlineExam.Core.Constants;
global using OnlineExam.Core.IServices;
global using System.Threading.RateLimiting;
global using OnlineExam.Core.IRepositories.Generic;
global using OnlineExam.Core.IServices.Provider;
global using OnlineExam.Core.IUnit;
global using OnlineExam.Infrastructure.Repositories.Generic;
global using OnlineExam.Infrastructure.Unit;