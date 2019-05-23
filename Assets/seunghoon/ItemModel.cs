using System;

namespace seunghoon
{
    [Serializable]
    public class ItemModel
    {
        public string itemName;

        ItemModel(string itemName)
        {
            this.itemName = itemName;
        }

        public static string parseItemToString(ItemNameEnum itemNameEnum)
        {
            switch (itemNameEnum)
            {
                case ItemNameEnum.DoubleScore:
                    return "DoubleScore";
                case ItemNameEnum.BigGoal:
                    return "BigGoal";
                default:
                    return "";
            }
        }

        public static ItemNameEnum ParseStringToItemNameEnum(string str)
        {
            switch (str)
            {
                case "DoubleScore":
                    return ItemNameEnum.DoubleScore;
                case "BigGoal" :
                    return ItemNameEnum.BigGoal;
                default:
                    return ItemNameEnum.None;
            }
        }
    }

    public enum ItemNameEnum
    {
        None, DoubleScore, BigGoal
    }
}