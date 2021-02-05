using System;
using System.IO;
using System.Collections.Generic;

namespace colorpicker
{
    public class Colorpicker
    {
        public int Pick()
        {
            int pick = 0;
            if (File.Exists("db/color.37"))
            {
                pick = int.Parse(File.ReadAllText("db/color.37"));
                Console.WriteLine("Henlo");
            }
            List<int> color = new List<int>
            {
                16711704,
                16753964,
                16777025,
                32795,
                249,
                8781949,
                14090864,
                7688087,
                14504,
                5623292,
                16295610,
                16777214,
                16295610,
                5623292,
                16718733,
                16767488,
                1815551,
                0,
                10724259,
                16777214,
                8388736
            };
            if (pick >= color.Count)
                pick = 0;
            Console.WriteLine(pick);
            int answer = pick;
            pick++;
            File.WriteAllText("db/color.37", pick.ToString());
            return color[answer];
            
        }
    }
}
