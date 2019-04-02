﻿using Mobile_Api.Interfaces;
using System;
using System.Collections.Generic;

namespace Mobile_Api.Models
{
    public class Album : BaseModel<Album>, IRvModel
    {
        public int Id { get; set; }

        public string AlbumType { get; set; }

        // JSON
        public string Artists { get; set; }

        // Json
        public string Copyrights { get; set; }

        //[Json] List<string>
        public string Genres { get; set; }

        public List<Image> Images { get; set; }

        public string Label { get; set; }

        public string Name { get; set; }

        //click count
        public long Popularity { get; set; }

        public DateTimeOffset ReleaseDate { get; set; }

        public long TotalTracks { get; set; }

        public List<Item> Songs { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastActiveTime { get; set; }

        public Album()
        {

        }

        public Album(bool fake)
        {

        }
    }
}
