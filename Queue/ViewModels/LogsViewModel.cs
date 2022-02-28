using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using System;

namespace Queue.ViewModels
{
    public class LogsViewModel
    {
        public BsonBinaryData _id { get; set; } = new BsonBinaryData(Guid.NewGuid(), GuidRepresentation.Standard);
        public DateTime datelog { get; set; }
        public string UserName { get; set; }
        public string company { get; set; }
        public string machine { get; set; }
        public string Module { get; set; }
        public string LogRegister { get; set; }
    }
}