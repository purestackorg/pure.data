using CommandLine;
using Pure.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PureGen
{
    public class Boostrapers
    {
        private static bool HasInit = false;
        public static void Init() {
            if (HasInit == false)
            {
                RegisterDefaultBoilerplates();
                HasInit = true;
            }
        }

        private static void RegisterDefaultBoilerplates() {
            NewBoilerplateManage.Register(new CSharpMvcNewBoilerplate());
        }

    }
}
 