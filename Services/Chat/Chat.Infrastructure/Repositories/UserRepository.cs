using System;
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
        public IUnitOfWork UnitOfWork { get; }

        public UserRepository(
            IUnitOfWork unitOfWork, 
            IDbConnectionFactory dbConnectionFactory,
            IMapper mapper)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _mapper = mapper;
            UnitOfWork = unitOfWork;
        }

        public void Add(User user)
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

        public async Task<User> GetAsync(string userId)
        {
            var sql = $@"SELECT * FROM Users
                        LEFT JOIN UserProfileMap ON UserProfileMap.UserId = Users.Id
                        LEFT JOIN Profiles ON UserProfileMap.ProfileId = Profiles.Id
                        WHERE Users.Id = @{nameof(userId)}
                        LIMIT 1";
            using (var connection = await _dbConnectionFactory.OpenConnectionAsync())
            {
                var user = await connection.QueryAsync<dynamic, dynamic, Profile, User>(
                    sql, 
                    (u, _, p) => new User(u.Id, u.UserName, u.Email, p),
                    new
                    {
                        userId
                    },
                    splitOn: "userId,id");
                return user.FirstOrDefault();
            }
        }

        public async Task<(bool, bool)> UserNameOrEmailExists(string userName, string email)
        {
            var userNameQuery = userName == null
                ? @"(SELECT COUNT(*) FROM Users WHERE false)"
                : $@"(SELECT COUNT(*) FROM Users WHERE Users.UserName = @{nameof(userName)})";

            var emailQuery = email == null
                ? @"(SELECT COUNT(*) FROM Users WHERE false)"
                : $@"(SELECT COUNT(*) FROM Users WHERE Users.Email = @{nameof(email)})";

            var sql = string.Join("UNION ALL", userNameQuery, emailQuery);
            using (var connection = await _dbConnectionFactory.OpenConnectionAsync())
            {
                var result = await connection.QueryAsync<bool>(sql, new
                {
                    userName,
                    email
                });
                var resultList = result.ToList();
                return (resultList[0], resultList[1]);
            }
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
