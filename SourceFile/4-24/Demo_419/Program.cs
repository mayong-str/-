namespace Demo
{
    internal static class Program
    {
        /// <summary>
        /// Ӧ�ó��������ڵ㡣
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool createNew;
            using (Mutex mutex = new Mutex(true, Application.ProductName, out createNew))
            {
                if (createNew)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Main());
                }
                else
                {
                    // �����Ѿ�����,��ʾ��ʾ���˳�
                    MessageBox.Show("Ӧ�ó����Ѿ�����!");
                }
            }
                
        }
    }
}