using System;
using System.Collections.Generic;
using System.Linq;

namespace ChestSystem
{
    public class Program
    {
        static void Main(string[] args)
        {
            var test = new WeightedRandom<string>();
            var data = new List<WeightedRandom<string>.ItemContainer>();
            var item = new WeightedRandom<string>.ItemContainer();
            item.TypeItem = "base";
            item.Weight = 0.6f;
            data.Add(item);
            var itemRare = new WeightedRandom<string>.ItemContainer();
            itemRare.TypeItem = "rare";
            itemRare.Weight = 0.3f;
            data.Add(itemRare);

            var typeName = "base";
            var iter = 1;
            while (iter < 16)
            {
                var name = test.GetRandomItems(data);
                Console.WriteLine("Iter: " + iter + " type: " + name.TypeItem);
                typeName = name.TypeItem; iter++;
            }
            Console.ReadLine();
        }
    }

    public class WeightedRandom<T>
    {
        private Random rand = new Random();
        private int attemptsCount = 0;
        private float coeffUpper = 0f;
        private float deltaCoeffUpper = 0.1f;

        public float StandartWeightForRareItem = 0.3f;
        public ItemContainer GetRandomItems(List<ItemContainer> items)
        {
            var weightSum = items.Sum(x => x.Weight);
            var value = (weightSum * ((rand.Next() / 1073741824.0f) - 1.0f));
            
            var current = 0f;

            if (attemptsCount >= 7)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].TypeItem == "rare")
                    {
                        var item = items[i];
                        item = SubWeightItem(item, 0.1f);
                        items[i] = item;
                        return items[i];
                    }
                    
                }
            }

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].TypeItem == "rare")
                {
                    coeffUpper += deltaCoeffUpper;
                    current += items[i].Weight + coeffUpper;
                }
                else
                {
                    current += items[i].Weight;
                }

                attemptsCount += 1;

                if (current > value)
                {
                    if (items[i].TypeItem == "rare")
                    {
                        var item = items[i];
                        item = SubWeightItem(item, 0.1f);
                        items[i] = item;
                    }
                    return items[i];
                }
            }
            throw new ArgumentException("Not items. Create items");
        }

        public struct ItemContainer
        {
            public T Item;
            public string TypeItem;
            public float Weight;
        }

        private ItemContainer SubWeightItem(ItemContainer item, float subFloat)
        {
            attemptsCount = 0;
            coeffUpper = 0;
            deltaCoeffUpper = 0;
            if (!(item.Weight <= 0))
                item.Weight -= subFloat;
            else
                item.Weight = StandartWeightForRareItem;
            return item;
        }
    }
}
