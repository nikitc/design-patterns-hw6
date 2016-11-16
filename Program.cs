using Example_06.ChainOfResponsibility;

namespace Example_06
{
    public class Program
    {
        static void Main(string[] args)
        {
            var bankomat = new Bancomat();
            bankomat.EnterAmount("160 $");
        }
    }
}
