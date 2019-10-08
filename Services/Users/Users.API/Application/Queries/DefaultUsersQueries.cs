﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Users.API.Dtos;
using Users.API.Infrastructure;
using Users.API.Infrastructure.Exceptions;

namespace Users.API.Application.Queries
{
    public class DefaultUsersQueries : IUsersQueries
    {
        private readonly IMapper _mapper;
        private DbConnectionInfo _dbConnectionInfo;

        public DefaultUsersQueries(IMapper mapper, IOptions<DbConnectionInfo> dbConnectionInfo)
        {
            _mapper = mapper;
            _dbConnectionInfo = dbConnectionInfo.Value;
        }

        public async Task<UserDto> GetUserByIdAsync(string id)
        {
            using (var connection = new MySqlConnection(_dbConnectionInfo.ConnectionString))
            {
                await connection.OpenAsync();

                var result = await connection.QueryAsync<dynamic>(
                    @"SELECT *
                         FROM AspNetUsers AS users 
                         WHERE users.Id = @Id
                         LIMIT 1",
                    new { Id = id }
                );

                var user = result.FirstOrDefault() 
                           ?? throw new UsersApiException("Could not fetch user", new[] { "User not found" }, 404);

                return _mapper.Map<UserDto>(user);
            }
        }
    }
}
