﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Pure.Data.Mapper;

//namespace DapperExtensions.Test.Data
//{
//    public class Multikey
//    {
//        public int Key1 { get; set; } 
//        public string Key2 { get; set; }
//        public string Value { get; set; }
//        //public DateTime Date { get; set; }
//    }

//    public class MultikeyMapper : ClassMapper<Multikey>
//    {
//        public MultikeyMapper()
//        {
//            Map(p => p.Key1).Key(KeyType.Identity);
//            Map(p => p.Key2).Key(KeyType.Assigned);
//            //Map(p => p.Date).Ignore();
//            AutoMap();
//        }
//    }
//}