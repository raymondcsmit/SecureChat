using System;
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
            var sql = $@"INSERT INTO Users (Id, UserName)
                        VALUES (@{nameof(user.Id)}, @{nameof(user.UserName)});";
            UnitOfWork.AddOperation(user, async connection =>
            {
                await connection.ExecuteAsync(sql, new
                {
                    user.Id,
                    Username = user.UserName
                });
            });
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
                    user.UserName, user.Email
                });
            });

            if (user.ProfileCreated)
            {
                var sql2 = $@"INSERT INTO Profiles (Age, Sex, Location)
                        VALUES (@{nameof(Profile.Age)}, @{nameof(Profile.Sex)}, @{nameof(Profile.Location)});
                        INSERT INTO UsersProfiles (UserId, ProfileId)
                        VALUES (@{nameof(User.Id)}, SELECT LAST_INSERT_ID());";
                UnitOfWork.AddOperation(user.Profile, async connection => 
                    await connection.ExecuteAsync(sql2, new
                    {
                        user.Profile.Age, user.Profile.Sex, user.Profile.Location
                    }));
            }
            else
            {
                var sql3 = $@"UPDATE Profiles SET
                            Age = @{nameof(Profile.Age)},
                            Sex = @{nameof(Profile.Sex)},
                            Location = @{nameof(Profile.Location)}
                        WHERE Users.Id = @{nameof(user.Id)}";
                UnitOfWork.AddOperation(user.Profile, async connection =>
                    await connection.ExecuteAsync(sql3, new
                    {
                        user.Profile.Age,
                        user.Profile.Sex,
                        user.Profile.Location
                    }));
            }
        }

        public async Task<User> GetAsync(string userId)
        {
            var sql = $@"SELECT * FROM Users
                        LEFT JOIN UserProfiles ON UserProfiles.UserId = Users.Id
                        JOIN Profiles ON UserProfiles.ProfileId = Profiles.Id
                        WHERE Users.Id = @{nameof(userId)}
                        LIMIT 1";
            using (var connection = await _dbConnectionFactory.OpenConnectionAsync())
            {
                var user = await connection.QueryAsync<dynamic, dynamic, Profile, User>(
                    sql, 
                    (u, _, p) => new User(u.Id, u.userName, u.email, p),
                    splitOn: "userId,id");
                return user == null ? null : _mapper.Map<User>(user);
            }
        }
    }
}
