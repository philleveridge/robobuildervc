using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Simulator
{
    class IniManager
    {
        string filename;
        Hashtable ht;

        public IniManager() 
        {
            ht = new Hashtable();
        }

        public bool Load(string f)
        {
            if (f == "") 
                filename = "profile.txt"; 
            else
                filename = f;

            // read file

            try
            {
                TextReader tr = new StreamReader(filename);
                string line = "";
                string name = "";
                string value = "";
                int p;

                while ((line = tr.ReadLine()) != null)
                {
                    line = line.Trim();
                    if ((p = line.IndexOf("#")) >= 0) line = line.Substring(0, p);
                    if ((p = line.IndexOf("//")) >= 0) line = line.Substring(0, p);
                    if ((p = line.IndexOf("'")) >= 0) line = line.Substring(0, p);

                    if (line.Length > 0)
                    {
                        line = expandString(line);

                        //Console.WriteLine(line);
                        int n = line.IndexOf('=');
                        if (n > 0)
                        {
                            name = line.Substring(0, n);
                            value = line.Substring(n + 1);
                        }
                        else
                        {
                            if (n == 0)
                            {
                                // continuation of previous line
                                value = value + line.Substring(1);
                            }
                            else
                            {
                                //name with no value
                                name = line;
                                value = "";
                            }
                        }
                        setParameter(name, value);
                    }
                }
                // close the stream
                tr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Can't open in file : " + f + "(" + e.ToString() + ")");
                return false;
            }

            return true;
        }

        public bool Save()
        {
            // this has not been written yet
            // create a writer and open the file
            TextWriter tw = new StreamWriter(filename + ".new");

            // write a line of text to the file
            tw.WriteLine(DateTime.Now);

            // close the stream
            tw.Close();
            return true;
        }

        string expandString(string x)
        {
            IDictionaryEnumerator en = ht.GetEnumerator();

            while (en.MoveNext())
            {
                string k = en.Key.ToString();
                string v = en.Value.ToString();
                x = x.Replace("$" + k, v);
            }
            return x;
        }

        public string getParameter(string p)
        {
            if (ht.ContainsKey(p.ToUpper()))
            {
                string k = p.ToUpper();
                return ht[k].ToString();
            }
            else
                return "";
        }

        public float getParameterEval(string p)
        {
            return evaluateString(getParameter(p));
        }

        public float[]  getParameterEvalArray(string p)
        {
            return processParams(getParameter(p));
        }

        float[] processParams(string x)
        {
            float[] ans;
            string[] t = x.Split(',');
            ans = new float[t.Length];
            for (int i = 0; i < t.Length; i++)
            {
                ans[i] = evaluateString(t[i]);
            }
            return ans;
        }

        public float evaluateString(string n)
        {
            float ans = 0;
            float temp = 0;
            char op = '\0';
            bool dec = false;
            for (int i = 0; i < n.Length; i++)
            {
                if (n[i] >= '0' && n[i] <= '9')
                {
                    ans = ans * 10 + (n[i] - '0');

                    if (dec)
                        ans = ans / 10;
                }
                if (n[i] == '.')
                {
                    dec = true;
                }
                switch (n[i])
                {
                    case '+':
                    case '-':
                        if (op == '+') ans = ans + temp;
                        if (op == '-') ans = ans - temp;
                        temp = ans;
                        ans = 0;
                        op = n[i];
                        dec = false;
                        break;
                }
            }
            if (op == '+') ans = ans + temp;
            if (op == '-') ans = temp - ans;
            return ans;
        }

        public void setParameter(string n, string v)
        {
            n = n.ToUpper();
            ht[n] = v;
        }
    }
}
