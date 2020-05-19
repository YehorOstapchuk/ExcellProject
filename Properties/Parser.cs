using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Parser Parsers = new Parser();
                //string s = "1 + 5 + 8 - 9 ";
                //  Console.WriteLine(Parsers.NoSpaces(s));
                // Console.WriteLine(Parsers.MinusToPlus(Parsers.NoSpaces(s)));
                string s = Console.ReadLine();
                // Console.WriteLine(Parsers.Parse("1/0"));
                // s = "(1+2)";
                //  Console.WriteLine(s.Substring(0, 3));
                Console.WriteLine(Parsers.Parse(s));
            }
            Console.ReadKey();
        }
    }

    class Number
    {
        public double value;
        public int first_index;
        public int second_index;
    }

    class Operation
    {
        public string value;
        public int priority;
        public int first_index;
        public int second_index;
    }


    class Parser
    {
        string operations = "+*/^incdecmoddiv";
        string numbers = "0123456789-";
        string alphabet = "ABCDEFGHIJKLMNOPQRSTVWXYZ";

        public string Parse(string s)
        {
            try
            {
                s = NoSpaces(s);
                s = MinusToPlus(s);

                while (true)
                {
                    string temp = "";
                    string temp2 = "";
                    string temp3 = "";
                    int first_bracket = -1;
                    int second_bracket = -1;


                    if (validation(s)) return s;

                    for (int i = 0; i < s.Length; i++) if (s[i] == '(') first_bracket = i;

                    if (first_bracket == -1) { s = Calculate(s); }
                    else
                    {
                        for (int i = first_bracket; i < s.Length; i++) if (s[i] == ')') { second_bracket = i; break; }

                        // Console.WriteLine(first_bracket);
                        // Console.WriteLine(second_bracket);

                        // Console.WriteLine(s.Substring(first_bracket + 1, second_bracket - 1));
                        for (int i = first_bracket + 1; i < second_bracket; i++)
                        {
                            temp += s[i];
                        }
                        // Console.WriteLine(temp);
                        temp = Calculate(temp);

                        //  temp = Calculate(s.Substring(first_bracket + 1, second_bracket - 1));
                        //  Console.WriteLine(temp);

                    }

                    for (int i = 0; i < s.Length; i++) if (i < first_bracket) temp2 += s[i];
                    for (int i = 0; i < s.Length; i++) if (i > second_bracket) temp3 += s[i];
                    s = temp2 + temp + temp3;

                }
            } catch (Exception) { return "Error"; }
        }
   

        public bool validation (string s)
        {
             string operations2 = operations + "()";
            for (int i =0; i<s.Length; i++)
            {
                if (operations2.Contains(s[i])) return false;
            }
            return true;
        }

        public int find_n (int index, List<Number> Arr)
        {
            for (int i =0; i< Arr.Count; i++)
            {
                if (Arr[i].first_index == index) return i;
                if (Arr[i].second_index == index) return i;
            }
            return -1;
        }


        public int find_o(int index, List<Operation> Arr)
        {
            for (int i = 0; i < Arr.Count; i++)
            {
                if (Arr[i].first_index == index) return i;
                if (Arr[i].second_index == index) return i;
            }
            return -1;
        }

        public string Calculate (string s)
        {
            List<Number> numbers_ = new List<Number>();
            List<Operation> operations_ = new List<Operation>();
            int i = 0;
            while (true)
            {
                if (i == s.Length) break;
                string temp = "";
                for (; i < s.Length; i++)
                {
                    if (operations.Contains(s[i])) break;
                    temp += s[i];
                }
                if (temp != "")
                {
                    Number temp1 = new Number();
                    temp1.value = double.Parse(temp);
                    temp1.second_index = i - 1;
                    temp1.first_index = (i - temp.Length);
                    numbers_.Add(temp1);
                }

                if (i == s.Length) break;
                string temp2 = "";
                for (; i < s.Length; i++)
                {
                    if (numbers.Contains(s[i])) break;
                    temp2 += s[i];
                }
                if (temp2 != "")
                {
                    Operation temp3 = new Operation();
                    temp3.value = temp2;
                    temp3.second_index = i - 1;
                    temp3.first_index = (i - temp2.Length);
                    operations_.Add(temp3);
                }
            }

            for (int j =0; j< operations_.Count; j++)
            {
                switch(operations_[j].value)
                {
                    case "^":
                        operations_[j].priority = 5;
                        break;

                    case "dec":
                        operations_[j].priority = 4;
                        break;
                    case "inc":
                        operations_[j].priority = 4;
                        break;

                    case "mod":
                        operations_[j].priority = 4;
                        break;
                    case "div":
                        operations_[j].priority = 4;
                        break;
                    case "/":
                        operations_[j].priority = 2;
                        break;
                    case "*":
                        operations_[j].priority = 2;
                        break;
                    case "+":
                        operations_[j].priority = 1;
                        break;
                    default :
                        operations_[j].priority = 0;
                        break;
                }
            }

            while (operations_.Count != 0)
            {
                int max = operations_[0].priority; int index = 0;
                for (int j = 0; j < operations_.Count; j++)
                {
                    if (operations_[j].priority > max) { max = operations_[j].priority; index = j; }
                }

             /*   for (int h =0; h< operations_.Count; h++)
                {
                    Console.WriteLine(operations_[h].first_index);
                    Console.WriteLine(operations_[h].second_index);
                    Console.WriteLine(operations_[h].value);
                    Console.WriteLine("------------");
                }


                for (int h = 0; h < numbers_.Count; h++)
                {
                    Console.WriteLine(numbers_[h].first_index);
                    Console.WriteLine(numbers_[h].second_index);
                    Console.WriteLine(numbers_[h].value);
                    Console.WriteLine("------------");
                }
                */

                switch (operations_[index].value)
                {

                    case "^":
                        {
                            Number temp = new Number();
                            int index1 = operations_[index].first_index;
                            int index2 = operations_[index].second_index;
                            int n1 = find_n(index1 - 1, numbers_);
                            int n2 = find_n(index2 + 1, numbers_);
                            temp.value =Math.Pow( numbers_[n1].value , numbers_[n2].value);
                            temp.first_index = numbers_[n1].first_index;
                            temp.second_index = numbers_[n2].second_index;
                            numbers_.RemoveAt(find_n(index1 - 1, numbers_));
                            numbers_.RemoveAt(find_n(index2 + 1, numbers_));
                            operations_.RemoveAt(index);
                            numbers_.Add(temp);
                        }
                        break;
                    case "/":
                        {
                            Number temp = new Number();
                            int index1 = operations_[index].first_index;
                            int index2 = operations_[index].second_index;
                            int n1 = find_n(index1 - 1, numbers_);
                            int n2 = find_n(index2 + 1, numbers_);
                            if (numbers_[n2].value == 0) throw new Exception();
                            temp.value = numbers_[n1].value / numbers_[n2].value;
                            temp.first_index = numbers_[n1].first_index;
                            temp.second_index = numbers_[n2].second_index;
                            numbers_.RemoveAt(find_n(index1 - 1, numbers_));
                            numbers_.RemoveAt(find_n(index2 + 1, numbers_));
                            operations_.RemoveAt(index);
                            numbers_.Add(temp);
                        }
                        break;
                    case "*":
                        {
                            Number temp = new Number();
                            int index1 = operations_[index].first_index;
                            int index2 = operations_[index].second_index;
                            int n1 = find_n(index1 - 1, numbers_);
                            int n2 = find_n(index2 + 1, numbers_);
                            temp.value = numbers_[n1].value * numbers_[n2].value;
                            temp.first_index = numbers_[n1].first_index;
                            temp.second_index = numbers_[n2].second_index;
                            numbers_.RemoveAt(find_n(index1 - 1, numbers_));
                            numbers_.RemoveAt(find_n(index2 + 1, numbers_));
                            operations_.RemoveAt(index);
                            numbers_.Add(temp);
                        }
                        break;
                    case "+":
                        {
                            Number temp = new Number();
                            int index1 = operations_[index].first_index;
                            int index2 = operations_[index].second_index;
                            int n1 = find_n(index1 - 1, numbers_);
                            int n2 = find_n(index2 + 1, numbers_);
                            temp.value = numbers_[n1].value + numbers_[n2].value;
                            temp.first_index = numbers_[n1].first_index;
                            temp.second_index = numbers_[n2].second_index;
                            numbers_.RemoveAt(find_n(index1 - 1, numbers_));
                            numbers_.RemoveAt(find_n(index2 + 1, numbers_));
                            operations_.RemoveAt(index);
                            numbers_.Add(temp);
                        }
                        break;

                    case "inc":
                        {
                            Number temp = new Number();
                           // int index1 = operations_[index].first_index;
                            int index2 = operations_[index].second_index;
                         
                          //  int n1 = find_n(index1 - 1, numbers_);
                            int n2 = find_n(index2 + 1, numbers_);
                            temp.value =  numbers_[n2].value+1;
                            temp.first_index = operations_[index].first_index;
                            temp.second_index = numbers_[n2].second_index;
                           // numbers_.RemoveAt(find_n(index1 - 1, numbers_));
                            numbers_.RemoveAt(find_n(index2 + 1, numbers_));
                            operations_.RemoveAt(index);
                            numbers_.Add(temp);
                        }
                        break;

                    case "dec":
                        {
                            Number temp = new Number();
                            // int index1 = operations_[index].first_index;
                            int index2 = operations_[index].second_index;

                            //  int n1 = find_n(index1 - 1, numbers_);
                            int n2 = find_n(index2 + 1, numbers_);
                            temp.value = numbers_[n2].value - 1;
                            temp.first_index = operations_[index].first_index;
                            temp.second_index = numbers_[n2].second_index;
                            // numbers_.RemoveAt(find_n(index1 - 1, numbers_));
                            numbers_.RemoveAt(find_n(index2 + 1, numbers_));
                            operations_.RemoveAt(index);
                            numbers_.Add(temp);
                        }
                        break;

                    case "mod":
                        {
                            Number temp = new Number();
                            int index1 = operations_[index].first_index;
                            int index2 = operations_[index].second_index;
                            int n1 = find_n(index1 - 1, numbers_);
                            int n2 = find_n(index2 + 1, numbers_);
                            temp.value = numbers_[n1].value % numbers_[n2].value;
                            temp.first_index = numbers_[n1].first_index;
                            temp.second_index = numbers_[n2].second_index;
                            numbers_.RemoveAt(find_n(index1 - 1, numbers_));
                            numbers_.RemoveAt(find_n(index2 + 1, numbers_));
                            operations_.RemoveAt(index);
                            numbers_.Add(temp);
                        }
                        break;

                    case "div":
                        {
                            Number temp = new Number();
                            int index1 = operations_[index].first_index;
                            int index2 = operations_[index].second_index;
                            int n1 = find_n(index1 - 1, numbers_);
                            int n2 = find_n(index2 + 1, numbers_);
                            temp.value = Math.Floor(numbers_[n1].value / numbers_[n2].value);
                            temp.first_index = numbers_[n1].first_index;
                            temp.second_index = numbers_[n2].second_index;
                            numbers_.RemoveAt(find_n(index1 - 1, numbers_));
                            numbers_.RemoveAt(find_n(index2 + 1, numbers_));
                            operations_.RemoveAt(index);
                            numbers_.Add(temp);
                        }
                        break;

                }

            }
            return (numbers_[0].value).ToString();
        }

        public string NoSpaces(string s)
        {
            string temp="";
            for(int i=0; i<s.Length; i++)
            {
                if (s[i] == ' ') continue;
                temp += s[i];
            }
            return temp;
        }

        public string MinusToPlus(string s)
        {
            string numbers2 = numbers + ")";
            for (int i =0; i< s.Length; i++)
            {
                if (s[i] == '-')
                {
            
                    if (i == 0) continue;
                    if (numbers2.Contains(s[i-1])) { s = s.Substring(0, i) + "+" + s.Substring(i); i++; }
                }
            }
            return s;
        }


    }


}
