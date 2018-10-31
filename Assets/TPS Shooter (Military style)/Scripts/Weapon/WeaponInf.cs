namespace TPSShooter
{

    [System.Serializable]
    public class WeaponInf
    {

        public string Tag;
        public int BulletsCount = 60;
        public int BulletsInMagazine = 15;

        public override int GetHashCode()
        {
            return Tag.Length;
        }

        public override bool Equals(object obj)
        {
            WeaponInf w = (WeaponInf)obj;
            return Tag.Equals(w.Tag);
        }

    }
}