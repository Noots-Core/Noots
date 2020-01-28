﻿using AutoMapper;
using MongoDB.Bson;
using Noots.DataAccess.Repositories;
using Shared.DTO.User;
using Shared.Mongo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Noots.BusinessLogic.Services
{
    public class UserService
    {
        private readonly UserRepository userRepository = null;
        private readonly IMapper mapper;
        public UserService(UserRepository userRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
        }

        public async Task Add(DTOUser user)
        {
            var bduser = mapper.Map<User>(user);
            await userRepository.Add(bduser);
        }

        public async Task<DTOUser> Get(ObjectId id)
        {
            var user  = await userRepository.Get(id);
            var bduser = mapper.Map<DTOUser>(user);
            return bduser;
        }
        public async Task<DTOUser> GetByEmail(string email)
        {
            var user = await userRepository.GetByEmail(email);
            var bduser = mapper.Map<DTOUser>(user);
            return bduser;
        }
    }
}
