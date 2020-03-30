﻿using Chat.API.Dtos;
using Helpers.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.API.Application.Queries
{
    public interface IFriendshipQueries
    {
        Task<(IEnumerable<FriendshipDto>, int)> GetFriendships(ISpecification<FriendshipDto> spec);

        Task<(IEnumerable<FriendshipDto>, int)> GetFriendshipsByUserId(string userId, QueryDto query);
    }
}
