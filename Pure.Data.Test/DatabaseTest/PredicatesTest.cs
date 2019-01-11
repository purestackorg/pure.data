using Expression2SqlTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pure.Data.Test.DatabaseTest
{ 
    public class PredicatesTest
    {
        public static void Test()
        {


            string title = "CRUDTest";
            Console.Title = title;

            CodeTimer.Time(title, 1, () =>
            {
                 
                Pres();
            });


            Console.Read();


        }


        public static void Pres()
        {
            var gp1 = Predicates.Group(GroupOperator.And);




            var predicate = Predicates.Field<UserInfo>(f => f.Name, Operator.Like, "Lead", true);

            gp1.Add(predicate);

            predicate.AddTo(gp1);
             

            var predicateProperty = Predicates.Property<PredicateTestEntity, PredicateTestEntity2>(f => f.Name, Operator.Le, f => f.Value, true);

            var predicateExists = Predicates.Exists<PredicateTestEntity2>(null, true);

            BetweenValues values = new BetweenValues();
            var predicateBetween = Predicates.Between<PredicateTestEntity>(f => f.Name, values, true);


            var predicateGroup = Predicates.Group(GroupOperator.Or, predicate, predicateExists);


            var sortGroup = Predicates.SortGroup();
            var Sort = Predicates.Sort<PredicateTestEntity>(f => f.Name, false);
            sortGroup.Add(Sort);
            sortGroup.Add(Sort);

        }
         
         

        public class PredicateTestEntity
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class PredicateTestEntity2
        {
            public int Key { get; set; }
            public string Value { get; set; }
        }
    }
}
