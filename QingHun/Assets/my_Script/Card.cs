using UnityEngine;

[System.Serializable]
public class Card
{
    public int id;          // 卡牌唯一ID
    public string type;     // 卡牌类型 (花色)
    public int number;      // 卡牌数字 (1-13, 14是小王, 15是大王)
    public Sprite image;    // 卡牌图片

    public Card(int id, string type, int number)
    {
        this.id = id;
        this.type = type;
        this.number = number;
    }

    // 获取卡牌的排序权值
    public int GetSortValue()
    {
        // Joker的特殊处理
        if (type == "Joker")
        {
            return number == 14 ? 1000 : 1001; // 小王1000，大王1001
        }

        // 普通牌的权值计算
        int baseValue = GetNumberValue() * 10; // 数字权值
        int suitValue = GetSuitValue();        // 花色权值
        return baseValue + suitValue;
    }

    // 获取数字的排序权值
    private int GetNumberValue()
    {
        if (number == 1) return 13;  // A
        if (number == 2) return 14;  // 2最大
        return number - 2;           // 3-K的权值
    }

    // 获取花色的排序权值 (0-3)
    private int GetSuitValue()
    {
        return type switch
        {
            "Spade" => 0,    // 黑桃
            "Heart" => 1,    // 红心
            "Diamond" => 2,  // 方块
            "Club" => 3,     // 梅花
            _ => 0
        };
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        Card other = (Card)obj;
        return id == other.id && type == other.type && number == other.number;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + id.GetHashCode();
            hash = hash * 23 + (type?.GetHashCode() ?? 0);
            hash = hash * 23 + number.GetHashCode();
            return hash;
        }
    }
}