 
namespace Pure.Data.Gen.CodeServer.RazorPaser
{
    public interface ITemplateParser
    {
        string Parse<T>(string template, string templateKey, T data );
        string Parse(string template, string templateKey, object data );
        string ParseFile(string filename, string templateKey, object data );
        string ParseAndOutput<T>(string outputFileName, string template, string templateKey, T data, string encoding = "utf-8", bool append = false );

        string ParseAndOutput(string outputFileName, string template, string templateKey, object data, string encoding = "utf-8", bool append = false );
        string ParseAndOutputFile(string outputFileName, string filename, string templateKey, object data, string encoding = "utf-8", bool append = false );

        void OutputResult(string outputFileName, string content, string encoding = "utf-8", bool append = false);

        bool Reset(string templateKey);
    }
}
