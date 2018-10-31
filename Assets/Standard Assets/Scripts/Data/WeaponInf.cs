
[System.Serializable]
public class WeaponInf {

    public WeaponInf(int magazineCapacity, int availableBullets, int bulletsInMag)
    {
        this.magazineCapacity = magazineCapacity;
        this.availableBullets = availableBullets;
        this.bulletsInMag = bulletsInMag;
    }

    int magazineCapacity;
    public int MagazineCapacity
    {
        get { return magazineCapacity; }
    }

    int availableBullets;
    public int AvailableBullets
    {
        get { return availableBullets; }
        set { availableBullets = value; }
    }

    int bulletsInMag;
    public int BulletsInMag
    {
        get { return bulletsInMag; }
        set { bulletsInMag = value; }
    }

}
