using System;
using System.Collections.Generic;
using System.Linq;

namespace Example_06.ChainOfResponsibility
{
    public enum CurrencyType
    {
        Eur,
        Dollar,
        Ruble
    }

    public interface IBanknote
    {
        CurrencyType Currency { get; }
        int Value { get; }
    }


    public class Bancomat
    {
        private readonly BanknoteHandler _handler;
        public Queue<string> Bills;
        public Bancomat()
        {
            _handler = new TenRubleHandler(null);
            _handler = new TenDollarHandler(_handler);
            _handler = new FiftyDollarHandler(_handler);
            _handler = new HundredDollarHandler(_handler);
        }

        public bool Validate(string banknote)
        {
            return _handler.Validate(banknote);
        }


        protected CurrencyType ParseCurrencyType(string currency)
        {
            var dictCurrency = new Dictionary<string, CurrencyType>()
            {
                {"$", CurrencyType.Dollar},
                {"Р", CurrencyType.Ruble}
            };

            return dictCurrency[currency];
        }

        public void EnterAmount(string query)
        {
            Bills = new Queue<string>();
            var data = query.Split().ToArray();
            var amount = int.Parse(data[0]);
            var currencyType = ParseCurrencyType(data[1]);
            if (IsCashOut(amount, currencyType, Bills))
                foreach (var valute in Bills)
                    Console.WriteLine("Выдано: " + valute);
            else
                Console.WriteLine("Incorrect amount");                
            
        }

        protected bool IsCashOut(int amount, CurrencyType currencyType, Queue<string> bill)
        {
            return  _handler.IsCashOut(amount, currencyType, bill);
        }
    }

    public abstract class BanknoteHandler
    {
        private readonly BanknoteHandler _nextHandler;

        protected BanknoteHandler(BanknoteHandler nextHandler)
        {
            _nextHandler = nextHandler;
        }

        public virtual bool Validate(string banknote)
        {
            return _nextHandler != null && _nextHandler.Validate(banknote);
        }

        public virtual bool IsCashOut(int amount, CurrencyType currencyType, Queue<string> bill)
        {
            return _nextHandler != null && _nextHandler.IsCashOut(amount, currencyType, bill);
        }

    }

    public class TenRubleHandler : BanknoteHandler, IBanknote
    {
        public int Value => 10;

        public CurrencyType Currency => CurrencyType.Ruble;
        
        public override bool Validate(string banknote)
        {
            if (banknote.Equals("10 Рублей"))
            {
                return true;
            }
            return base.Validate(banknote);
        }

        public override bool IsCashOut(int amount, CurrencyType currencyType, Queue<string> bill)
        {
            if (Currency != currencyType)
                return base.IsCashOut(amount, currencyType, bill);

            while(amount >= Value)
            {
                bill.Enqueue(Value + " рублей");
                amount -= Value;
            }

            return amount == 0 || base.IsCashOut(amount, currencyType, bill);
        }

        public TenRubleHandler(BanknoteHandler nextHandler) : base(nextHandler)
        { }
    }

    public abstract class DollarHandlerBase : BanknoteHandler, IBanknote
    {
        public override bool Validate(string banknote)
        {
            if (banknote.Equals($"{Value}$"))
            {
                return true;
            }
            return base.Validate(banknote);
        }

        public override bool IsCashOut(int amount, CurrencyType currencyType, Queue<string> bill)
        {
            if (Currency != currencyType || amount < Value)
                return base.IsCashOut(amount, currencyType, bill);

            while (amount >= Value)
            {
                bill.Enqueue(Value + " $");
                amount -= Value;
            }

            return amount == 0 || base.IsCashOut(amount, currencyType, bill);

        }

        public CurrencyType Currency => CurrencyType.Dollar;
        public abstract int Value { get; }

        protected DollarHandlerBase(BanknoteHandler nextHandler) : base(nextHandler)
        {
        }
    }

    public class HundredDollarHandler : DollarHandlerBase
    {
        public override int Value => 100;

        public HundredDollarHandler(BanknoteHandler nextHandler) : base(nextHandler)
        { }
    }

    public class FiftyDollarHandler : DollarHandlerBase
    {
        public override int Value => 50;

        public FiftyDollarHandler(BanknoteHandler nextHandler) : base(nextHandler)
        { }
    }

    public class TenDollarHandler : DollarHandlerBase
    {
        public override int Value => 10;

        public TenDollarHandler(BanknoteHandler nextHandler) : base(nextHandler)
        { }
    }
}
