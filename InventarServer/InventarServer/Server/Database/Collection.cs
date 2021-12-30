using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventarServer
{
    class Collection
    {
        public IMongoCollection<BsonDocument> MongoCollection { get; set; }

        public Collection(IMongoCollection<BsonDocument> _collection)
        {
            MongoCollection = _collection;
        }

        public BsonDocument FindOne(string _filterCriterium, string _name)
        {
            var filter = Builders<BsonDocument>.Filter.Eq(_filterCriterium, _name);
            return MongoCollection.Find(filter).FirstOrDefault();
        }

        public void AddOne(BsonDocument _doc)
        {
            MongoCollection.InsertOne(_doc);
        }

        public List<BsonDocument> GetAll()
        {
            return MongoCollection.Find(_ => true).ToList();
        }

        public void UpdateEntry(string _filterCriterium, string _name, BsonDocument _newDoc)
        {
            var filter = Builders<BsonDocument>.Filter.Eq(_filterCriterium, _name);
            MongoCollection.ReplaceOne(filter, _newDoc);
        }
    }
}
