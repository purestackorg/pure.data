using System; 
namespace Pure.Data.Pooling
{
    public class Evictor
    {
        private Action action;

        /// <summary>初始化 <see cref="T:System.Object" /> 类的新实例。</summary>
        public Evictor(Action action)
        {
            this.action = action ?? throw new ArgumentNullException(nameof(action));
        }
        
        public void Run()
        {
            this.action();
        }
    }
}
