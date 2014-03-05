using System;
using System.Collections.Generic;
using System.Linq;

namespace PrefixCalc
{

		class Program {
				static IDictionary<string,double> mem = new Dictionary<string,double>(); //binary search tree

				static void Main(string[] args){

						while (true) {
								string s = Console.ReadLine();
								if (s == null)
										break; //EOF
								try {
										Console.WriteLine(Eval(s));

								} catch (Exception ex) {
										Console.WriteLine(ex.Message);
								}
						}

				}
		 
				static double Eval(string s){
						var t = Step(s.Split(new string[] {" ", "\t", ",", "(", ")"}, StringSplitOptions.RemoveEmptyEntries));
						mem["_"] = t.Item1; //remember last result
						return t.Item1;
				}

				static Tuple<double, IEnumerable<string>> Step(IEnumerable<string> l){
						//consume and make into (number, rest of stream)

						Func<double, IEnumerable<string>, Tuple<double, IEnumerable<string>>> t = 
				        (d, s) => new Tuple<double, IEnumerable<string>>(d, s);

						Func<double,double,double> op = (a, b) => 0; //just pass-like implicit; should never happen 

						string head = l.First();

						switch (head) {
								case "*":
										op = (a, b) => a * b;
										break;
								case "+":
										op = (a, b) => a + b;
										break;
								case "/":
										op = (a, b) => a / b;
										break;
								case "-":
										op = (a, b) => a - b;
										break;
								case "pow":
										op = Math.Pow;
										break;
								case "log":
										op = Math.Log;
										break;
								case "==":
										op = (a, b) => a == b ? 1 : 0;
										break;

						//specials (does not need to just take 2 int args)

								case "sqrt": //takes only one argument
										var arg = Step(l.Skip(1)); //skip the operator
										return t(Math.Sqrt(arg.Item1), arg.Item2);

								case "def": //first arg is name
										string name = l.Skip(1).First(); //one after 'def' (identifier)
										var tt = Step(l.Skip(2));        //compute the second argument (skip 2: 1def 2name 3_data_ )
										double orig = mem.ContainsKey(name) ? mem[name] : tt.Item1; //get optional original value, current if none
										mem[name] = tt.Item1;
										return t(orig, tt.Item2); 

								default:
										double num; //if number: use that, otherwise try mem
										return t(double.TryParse(head, out num) ? num : mem[head], l.Skip(1)); //this Skip is used in multi args calls, not first
						}

						//non-specials end up here:

						var t1 = Step(l.Skip(1)); //skip the operator
						var t2 = Step(t1.Item2);
						return t(op(t1.Item1, t2.Item1), t2.Item2);

				}
		}
}


