﻿using MongoDB.Bson;
using Shared.Mongo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Noots.DataAccess.InterfacesRepositories
{
    public interface INootRepository
    {
        Task<MongoNoot> GetFullNoot(ObjectId id);
    }
}
