
public class MainBuilding : Building
{
    protected override void GetTotalDamage()
    {
        if (isÀttacked == true)
        {
            buildingHP += totalDamage;
            if (currentSide == Side.Player) 
            {
                if (buildingHP <= 0)
                {
                    lastSide = currentSide;
                    currentSide = Side.Neutral;
                    buildingHP = 0;
                    isÑaptured = false;
                    ÑhangeBuildingLevelToStart();
                    OnBuildingSideChanged(this);
                }
            }
            else if (currentSide == Side.Enemy)
            {
                if (buildingHP >= 0)
                {
                    lastSide = currentSide;
                    currentSide = Side.Neutral;
                    buildingHP = 0;
                    isÑaptured = false;
                    ÑhangeBuildingLevelToStart();
                    OnBuildingSideChanged(this);
                }
            }
            else // side == Side.Neutral
            {
                if (buildingHP > maxBuildingHP)
                {
                    lastSide = currentSide;
                    currentSide = Side.Player;
                    if (currentSide != startSide)
                        UIManager.instance.ShowWinPanel();

                    buildingHP = maxBuildingHP;
                    isÑaptured = true;
                    OnBuildingSideChanged(this);
                }
                else if (buildingHP < minBuildingHP)
                {
                    lastSide = currentSide;
                    currentSide = Side.Enemy;
                    if (currentSide != startSide)
                        UIManager.instance.ShowLosePanel();
                    buildingHP = minBuildingHP;
                    isÑaptured = true;
                    OnBuildingSideChanged(this);
                }
            }
            isDamaged = true;
            stunTime = 0;
            totalDamage = 0;
            isÀttacked = false;
        }
    }
    
}
