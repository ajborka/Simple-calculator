using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace Simple_calculator
{
    class Program
    {
        //The container used to hold the available parts.
        private CompositionContainer _container;

        //The imported calculator.
        [Import(typeof(ICalculator))]
        public ICalculator calculator;

        private Program()
        {
            //Catalog to hold multiple plugin sources.
            var catalog = new AggregateCatalog();

            //Add the parts available in the local assembly.
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(Program).Assembly));

            //Import the available parts into the container.
            _container = new CompositionContainer(catalog);
            
            //Fill the container with the available parts.
            try
            {
                this._container.ComposeParts(this);
            } //End try block
            catch(CompositionException e)
            {
                Console.WriteLine(e.Message);
            } //End catch block
        } //End Program constructor.

        static void Main(string[] args)
        {

            var p = new Program();
            string s;

            while(true)
            {
                Console.Write("Enter command: ");
                s = Console.ReadLine();
                Console.WriteLine(p.calculator.Calculate(s));
            } //End while loop.
        } //End Main method.
    } //End Program class.

//ICalculator interface.
public interface ICalculator
    {
        string Calculate(string input);
    } //End ICalculator interface.

    public interface IOperation
    {
        int Operate(int left, int right);
    } //End IOperation interface.

    public interface IOperationData
    {
        char Symbol { get; }
    } //End IOperationData interface.
      //Addition
      [Export(typeof(IOperation))]
      [ExportMetadata("Symbol", '+')]
    public class add:IOperation
    {
        public int Operate(int left, int right)
        {
            return left + right;
        } //End Operate method.
    } //End add class.

    //Subtract operation.
    [Export(typeof(IOperation))]
    [ExportMetadata("Symbol", '-')]
    public class Subtract: IOperation
    {
        public int Operate(int left, int right)
        {
            return left - right;
        } //End Operate method.
    } //End Subtract class.
    //Class that implements ICalculator.
[Export(typeof(ICalculator))]
public class BasicCalculator: ICalculator
    {
        [ImportMany]
        IEnumerable<Lazy <IOperation, IOperationData>> operations;

            public string Calculate(string input)
        {

            int left;
            int right;
            char operation;

            //Find the operator.
            int operatorIndex = FindFirstNonDidgit(input);

            //Check to see if we have an operator.
            if(operatorIndex < 0)
            {
                return "Illegal operation.";
            } //End operator check.

            //Attempt to split the string into sections.
            try
            {
                left = int.Parse(input.Substring(0, operatorIndex));
                right = int.Parse(input.Substring(operatorIndex + 1));
            } //End try block
        catch
            {

                return "Cannot parse command";
            } //End catch block.

            //Assuming everything worked, perform the calculation.
            operation = input[operatorIndex];
            foreach(Lazy<IOperation, IOperationData> i in operations)
            {
                if(i.Metadata.Symbol.Equals(operation))
                {
                    return i.Value.Operate(left, right).ToString();
                } //End check for operation.
                            } //End foreach loop.
            return "Operation not found";
        } //End Calculate method.

        //Find the operator.
        private int FindFirstNonDidgit(string input)
        {
            for(int i = 0; i < input.Length; i++)
            {
                if(!(char.IsDigit(input[i])))
                {
                    return i;
                }
            } //End for loop.
            return -1;
        } //End FindFirstNonDigit method.
    } //End BasicCalculator class.
} //End simple_calculator namespace.
