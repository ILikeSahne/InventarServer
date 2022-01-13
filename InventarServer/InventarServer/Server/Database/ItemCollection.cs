using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventarServer
{
    class ItemCollection
    {
        public Database Database { get; set; }
        public BsonDocument Collection { get; set; }

        public ItemCollection(Database _db, BsonDocument _collection)
        {
            Database = _db;
            Collection = _collection;
        }

        public ItemCollection(Database _db, string _itemCollectionName)
        {
            Database = _db;
            Collection = Database.GetCollection("items").FindOne("name", _itemCollectionName);
        }

        public string GetName()
        {
            if (Collection == null)
                return "";
            return Collection.GetValue("name").AsString;
        }

        public string GetPermission()
        {
            if (Collection == null)
                return null;
            return Collection.GetValue("permission").AsString;
        }

        public BsonArray GetItems()
        {
            if (Collection == null)
                return null;
            return Collection.GetValue("items").AsBsonArray;
        }

        public void AddItem(Item _i)
        {
            if (Collection == null)
                return;
            var items = Collection.GetValue("items").AsBsonArray;
            items.Add(_i.GetItemAsBson());

            Database.GetCollection("items").UpdateEntry("name", _i.ItemCollectionName, Collection);
        }

        public static BsonDocument CreateNew(string _name, string _perm)
        {
            BsonDocument doc = new BsonDocument();
            doc.Add("name", _name);
            doc.Add("items", new BsonArray());
            doc.Add("permission", _perm);
            return doc;
        }
    }
}
