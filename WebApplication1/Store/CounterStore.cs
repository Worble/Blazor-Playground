using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Store
{
    public class CounterStore : IStore
    {
        //Model
        private bool initialized = false;

        public double CurrentCount { get; private set; } = 0;
        public double CountPerSecond { get; private set; } = 0;
        public List<Building> Buildings { get; private set; } = new List<Building>()
        {
        new Building(1, 1, "Building One", 0, 10),
        new Building(2, 10, "Building Two", 0, 50)
        };
        public int MsModifier { get; private set; } = 100;

        public event EventHandler StateHasChanged;

        public void Initialize()
        {
            if (!initialized)
            {
                TimeSpan startTimeSpan = TimeSpan.Zero;
                TimeSpan periodTimeSpan = TimeSpan.FromMilliseconds(MsModifier);

                System.Threading.Timer timer = new System.Threading.Timer((e) =>
                {
                    this.Update();
                }, null, startTimeSpan, periodTimeSpan);
                initialized = true;
            }
        }

        //UPDATE
        public int GetCount()
        {
            return (int)Math.Round(this.CurrentCount);
        }

        public void IncrementCount()
        {
            this.CurrentCount++;
        }

        public void Update()
        {
            double amountPerSecond = 0;
            foreach (var building in this.Buildings)
            {
                amountPerSecond += building.CalculateClicksPerSecond();
            }
            this.CountPerSecond = amountPerSecond;
            this.CurrentCount += CountPerSecond / (1000 / this.MsModifier);
            StateChanged(new EventArgs());
        }

        public void BuyBuilding(int buildingId)
        {
            var building = Buildings.FirstOrDefault(e => e.Id == buildingId);
            if (building.Cost <= this.CurrentCount)
            {
                building.BuyOne();
                this.CurrentCount -= building.Cost;
            }
        }
        protected virtual void StateChanged(EventArgs args)
        {
            StateHasChanged?.Invoke(this, args);
        }

        //OTHER MODELS
        public class Building
        {
            //CONSTRUCTOR
            public Building(int id, int amountPerSecond, string name, int amountOwned, int cost)
            {
                this.Id = id;
                this.AmountPerSecond = amountPerSecond;
                this.Name = name;
                this.AmountOwned = AmountOwned;
                this.Cost = cost;
            }

            //MODEL
            public int Id { get; private set; }
            public int AmountPerSecond { get; private set; }
            public string Name { get; private set; }
            public int AmountOwned { get; private set; }
            public int Cost { get; set; }

            //UPDATE
            public void BuyOne()
            {
                this.AmountOwned++;
            }

            public double CalculateClicksPerSecond()
            {
                return this.AmountOwned * this.AmountPerSecond;
            }
        }
    }
}
