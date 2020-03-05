using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Chat.Domain.AggregateModel.UserAggregate;
using Chat.Domain.SeedWork;
using Dapper;
using Profile = Chat.Domain.AggregateModel.UserAggregate.Profile;

namespace Chat.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly IMapper _mapper;
        private readonly IFriendshipRequestRepository _friendshipRequestRepository;
        public IUnitOfWork UnitOfWork { get; }

        public UserRepository(
            IUnitOfWork unitOfWork, 
            IDbConnectionFactory dbConnectionFactory,
            IMapper mapper,
            IFriendshipRequestRepository friendshipRequestRepository)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _mapper = mapper;
            _friendshipRequestRepository = friendshipRequestRepository;
            UnitOfWork = unitOfWork;
        }

        public void Create(User user)
        {
            var sql = $@"INSERT INTO Users (Id, UserName, Email)
                        VALUES (@{nameof(user.Id)}, @{nameof(user.UserName)}, @{nameof(user.Email)});";
            UnitOfWork.AddOperation(user, async connection =>
            {
                await connection.ExecuteAsync(sql, new
                {
                    user.Id,
                    user.UserName,
                    user.Email
                });
            });

            if (user.Profile != null)
            {
                AddProfile(user.Id, user.Profile);
            }
        }

        public void Update(User user)
        {
            var sql1 = $@"UPDATE Users SET
                            UserName = @{nameof(user.UserName)},
                            Email = @{nameof(user.Email)}
                        WHERE Users.Id = @{nameof(user.Id)}";
            UnitOfWork.AddOperation(user, async connection =>
            {
                await connection.ExecuteAsync(sql1, new
                {
                    user.UserName,
                    user.Email,
                    user.Id
                });
            });

            if (user.HasFlag(User.Flags.ProfileAdded))
            {
                AddProfile(user.Id, user.Profile);
                user.ClearFlag(User.Flags.ProfileAdded);
            }
            else if (user.HasProfile)
            {
                UpdateProfile(user.Profile);
            }
        }

        public async Task<User> GetByIdAsync(string userId)
        {
            var friendshipRequests = await _friendshipRequestRepository.GetByUserIdAsync(userId);

            var sql = $@"SELECT * FROM Users
                        LEFT JOIN UserProfileMap ON UserProfileMap.UserId = Users.Id
                        LEFT JOIN Profiles ON UserProfileMap.ProfileId = Profiles.Id
                        WHERE Users.Id = @{nameof(userId)}
                        LIMIT 1";

            using (var connection = await _dbConnectionFactory.OpenConnectionAsync())
            {
                var result = await connection.QueryAsync<dynamic, dynamic, Profile, (dynamic, Profile)>(
                    sql,
                    (u, _, p) => (u, p),
                    new
                    {
                        userId
                    },
                    splitOn: "userId,id");
                var (user, profile) = result.FirstOrDefault();

                return user == null ? null : new User(user.Id, user.UserName, user.Email, profile, friendshipRequests);
            }
        }

        public void DeleteById(string id)
        {
            throw new NotImplementedException();
        }

        private void AddProfile(string userId, Profile profile)
        {
            var sql = $@"INSERT INTO Profiles (Age, Sex, Location)
                        VALUES (@{nameof(Profile.Age)}, @{nameof(Profile.Sex)}, @{nameof(Profile.Location)});
                        INSERT INTO UserProfileMap (UserId, ProfileId)
                        VALUES (@{nameof(userId)}, LAST_INSERT_ID());";
            UnitOfWork.AddOperation(profile, async connection =>
                await connection.ExecuteAsync(sql, new
                {
                    profile.Age,
                    profile.Sex,
                    profile.Location,
                    userId
                }));
        }

        private void UpdateProfile(Profile profile)
        {
            var sql = $@"UPDATE Profiles SET
                            Age = @{nameof(Profile.Age)},
                            Sex = @{nameof(Profile.Sex)},
                            Location = @{nameof(Profile.Location)}
                        WHERE Profiles.Id = @{nameof(profile.Id)}";
            UnitOfWork.AddOperation(profile, async connection =>
                await connection.ExecuteAsync(sql, new
                {
                    profile.Age,
                    profile.Sex,
                    profile.Location,
                    profile.Id
                }));
        }
    }
}
