namespace Move_Files
{
    class Program
    {
        static void Main(string[] args)
        {
            MoveFilesEngine mfe = new MoveFilesEngine();
            mfe.Setup();
            mfe.Start();
        }
    }
}
