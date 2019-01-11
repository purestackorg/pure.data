
using System.IO;
using Pure.Data.Migration.Framework;

namespace Pure.Data.Migration.Tools
{
	public class SchemaDumper
	{
	    private readonly ITransformationProvider _provider;
		
		public SchemaDumper(string provider, string connectionString)
		{
			_provider = ProviderFactory.Create(provider, connectionString);
		}
		
		public string Dump()
		{
			StringWriter writer = new StringWriter();
			
			writer.WriteLine("using Migrator;\n");
			writer.WriteLine("[Migration(1)]");
			writer.WriteLine("public class SchemaDump : Migration");
			writer.WriteLine("{");
			writer.WriteLine("\tpublic override void Up()");
			writer.WriteLine("\t{");
			
			foreach (string table in _provider.GetTables())
			{
				writer.WriteLine("\t\tDatabase.AddTable(\"{0}\",", table);
				foreach (Column column in _provider.GetColumns(table))
				{
					writer.WriteLine("\t\t\tnew Column(\"{0}\", typeof({1})),", column.Name, column.Type);
				}
				writer.WriteLine("\t\t);");
			}
			
			writer.WriteLine("\t}\n");
			writer.WriteLine("\tpublic override void Down()");
			writer.WriteLine("\t{");
			
			foreach (string table in _provider.GetTables())
			{
				writer.WriteLine("\t\tDatabase.RemoveTable(\"{0}\");", table);
			}
			
			writer.WriteLine("\t}");
			writer.WriteLine("}");
			
			return writer.ToString();
		}
		
		public void DumpTo(string file)
		{
			using (StreamWriter writer = new StreamWriter(file))
			{
				writer.Write(Dump());
			}
		}
	}
}