
/**
 * Created by davidvronay on 9/5/16.
 */
using System;

namespace Phabrik.Core
{
    public class StructureObj
    {
        public long Id { get; set; }
        public SectorObj sector { get; set; }
        public long sectorId { get; set; }
        public int xLoc { get; set; }
        public int yLoc { get; set; }
        public int xSize { get; set; }
        public int ySize { get; set; }
        public int curPop { get; set; }
        public int maxPop { get; set; }
        public int curHP { get; set; }
        public int maxHP { get; set; }
        public int minPowerNeed { get; set; }
        public int minPopNeed { get; set; }
        public int solidStorageSpace { get; set; }
        public int gasStorageSpace { get; set; }
        public int foodStorageSpace { get; set; }
        public int liquidStorageSpace { get; set; }
        public int energyStorageSpace { get; set; }
        public int strangeStorageSpace { get; set; }
        public int maxSolidStorageSpace { get; set; }
        public int maxGasStorageSpace { get; set; }
        public int maxFoodStorageSpace { get; set; }
        public int maxLiquidStorageSpace { get; set; }
        public int maxEnergyStorageSpace { get; set; }
        public int maxStrangeStorageSpace { get; set; }
        public DateTime creationDate { get; set; }
        public DateTime lastTick { get; set; }
        public long ownerId { get; set; }
        public double physicalDefense { get; set; }
        public double energyDefense { get; set; }
        public bool isVacuumSafe { get; set; }
        public bool isRadiationSafe { get; set; }
        public bool isPublic { get; set; }
        public long structureTypeId { get; set; }

        public string nickname { get; set; }
        

        public StructureObj()
        {
			// empty
			creationDate = DateTime.Now;
			lastTick = DateTime.Now;
        }

        public static StructureObj 	Instantiate(StructureTypeObj template)
        {
            StructureObj newObj = new StructureObj();

            newObj.xLoc = 0;
            newObj.yLoc = 0;
            newObj.xSize = template.width;
            newObj.ySize = template.height;
            newObj.structureTypeId = template.Id;
            newObj.nickname = template.structurename;


            return newObj;
        }

        public StructureObj(StructureObj master)
        {
            structureTypeId = master.structureTypeId;
            isPublic = master.isPublic;
            xLoc = master.xLoc;
            yLoc = master.yLoc;
            xSize = master.xSize;
            ySize = master.ySize;
            curPop = master.curPop;
            maxPop = master.maxPop;
            curHP = master.curHP;
            maxHP = master.maxHP;
            minPowerNeed = master.minPowerNeed;
            solidStorageSpace = master.solidStorageSpace;
            gasStorageSpace = master.gasStorageSpace;
            foodStorageSpace = master.foodStorageSpace;
            liquidStorageSpace = master.liquidStorageSpace;
            energyStorageSpace = master.energyStorageSpace;
            strangeStorageSpace = master.strangeStorageSpace;
            maxSolidStorageSpace = master.maxSolidStorageSpace;
            maxGasStorageSpace = master.maxGasStorageSpace;
            maxFoodStorageSpace = master.maxFoodStorageSpace;
            maxLiquidStorageSpace = master.maxLiquidStorageSpace;
            maxEnergyStorageSpace = master.maxEnergyStorageSpace;
            maxStrangeStorageSpace = master.maxStrangeStorageSpace;
            minPopNeed = master.minPopNeed;
            creationDate = new DateTime();
            lastTick = master.lastTick;
            ownerId = master.ownerId;
            physicalDefense = master.physicalDefense;
            energyDefense = master.energyDefense;
            isVacuumSafe = master.isVacuumSafe;
            isRadiationSafe = master.isRadiationSafe;
        }

        public string imageURL
        {
            get
            {
                string url = "structures/";
                url += System.Uri.EscapeUriString(nickname.Replace(' ','_'));
                url = PhabrikServer.BaseImageUrl + url + ".png";
                return url;
            }
        }

    }
}
