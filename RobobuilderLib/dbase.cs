using System;
using System.Collections.Generic;

namespace RobobuilderLib
{
    public class dbase
    {
        // private data
        public struct db
        {
            public double fit;
            public int[] inputs;
            public int[] outputs;
        };

        List<db> database = new List<db>();


        public dbase()
        {
            clear_db();
        }

        public db getRow(int index)
        {
            return database[index];
        }

        public void clear_db()
        {
            database.Clear();
        }

        public int match_dbi(int[] b, out double min)
        {
            min = 0.0;
            if (database.Count < 2) return -1;

            min = vectors.normal(vectors.sub(database[0].inputs, b));
            int r = 0;

            for (int i = 1; i < database.Count; i++)
            {
                double t = vectors.normal(vectors.sub(database[i].inputs, b));
                if (t < min)
                {
                    min = t;
                    r = i;
                }
            }
            return r;
        }

        public int find_db(int[] a)
        {
            int r = -1;
            for (int i = 0; i < database.Count; i++)
            {
                if (vectors.equals(a, database[i].inputs))
                    return i;
            }
            return r;
        }

        public void update_db(int index, double x)
        {
            db entry = new db();
            entry.fit = x;
            entry.inputs = database[index].inputs;
            entry.outputs = database[index].outputs;

            update_db(index, entry);
        }

        public void update_db(int index, int[] outp)
        {
            db entry = new db();
            entry.fit = database[index].fit;
            entry.inputs = database[index].inputs;
            entry.outputs = outp;

            update_db(index, entry);
        }

        private void update_db(int index, db entry)
        {
            database.RemoveAt(index);
            database.Insert(index, entry);
        }

        public void store_db(int[] a, int[] b, double x)
        {
            db entry = new db();
            entry.fit = x;
            entry.inputs = a;
            entry.outputs = b;

            if (find_db(a) >= 0)
            {
                Console.WriteLine("already exists");
            }
            else
            {
                database.Add(entry);
            }
        }

        private int compare(db a, db b)
        {
            return (vectors.compare(a.inputs, b.inputs));
        }

        public void show_db()
        {
            database.Sort(compare);
            for (int i = 0; i < database.Count; i++)
            {
                Console.WriteLine("[{0}] {3:0.0} ({1}) ({2})", i,
                    vectors.str(database[i].inputs),
                    vectors.str(database[i].outputs),
                    database[i].fit);
            }
        }


    }
}
