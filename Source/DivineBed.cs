using RimWorld;
using Verse;

namespace DivineBedMod
{
    // Класс свойств (рекомендуемый способ подключения)
    public class CompProperties_CheatBed : CompProperties
    {
        public CompProperties_CheatBed()
        {
            this.compClass = typeof(CompCheatBed);
        }
    }

    public class CompCheatBed : ThingComp
    {
        // Используем CompTickRare (срабатывает каждые 250 тиков)
        // Для этого в XML должен быть <tickerType>Rare</tickerType>
        public override void CompTickRare()
        {
            base.CompTickRare();
            RestoreNeeds();
        }

        // На случай, если в XML останется <tickerType>Normal</tickerType>,
        // добавим проверку и в обычный тик, но с оптимизацией
        public override void CompTick()
        {
            base.CompTick();
            if (this.parent.IsHashIntervalTick(250))
            {
                RestoreNeeds();
            }
        }

        private void RestoreNeeds()
        {
            if (this.parent is Building_Bed bed)
            {
                foreach (Pawn pawn in bed.CurOccupants)
                {
                    if (pawn?.needs == null) continue;

                    // Восстанавливаем еду
                    if (pawn.needs.food != null)
                        pawn.needs.food.CurLevelPercentage = 1f;

                    // Восстанавливаем комфорт
                    if (pawn.needs.comfort != null)
                        pawn.needs.comfort.CurLevelPercentage = 1f;

                    // Учитывая описание вашей кровати про "мгновенный отдых",
                    // добавим и восстановление сна (Rest)
                    if (pawn.needs.rest != null)
                        pawn.needs.rest.CurLevelPercentage = 1f;
                }
            }
        }
    }
}