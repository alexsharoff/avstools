using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace AVS.Tools.Mongo
{
    public class MongoWCF
    {
        protected MongoGridFS GridFS
        {
            get;
            set;
        }
        protected MongoServer Mongo
        {
            get;
            set;
        }
        protected MongoDatabase Database
        {
            get;
            set;
        }

        public void Connect(string host, ushort port, string database)
        {
            if (Mongo != null)
            {
                try
                {
                    Mongo.Disconnect();
                }
                catch { }
            }
            Mongo = MongoServer.Create(string.Format("mongodb://{0}:{1}", host, port));
            try
            {
                Mongo.Connect();
            }
            catch (Exception)
            {
                Mongo = null;
                throw;
            }
            Database = Mongo.GetDatabase(database);
            GridFS = new MongoGridFS(Database);
        }
    }
}
