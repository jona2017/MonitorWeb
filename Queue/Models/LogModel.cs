using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Queue.Models
{
    public class LogModel
    {
        public BsonBinaryData _id { get; set; } = new BsonBinaryData(Guid.NewGuid(), GuidRepresentation.Standard);
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public string IdEmpresa { get; set; }
    }
}