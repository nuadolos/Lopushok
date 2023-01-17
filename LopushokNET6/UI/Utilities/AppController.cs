using Lopushok.LopushokDataBase;
using System.Windows.Controls;

namespace Lopushok.UI.Utilities;

#nullable disable
static class AppController
{
    public static Frame AppFrame { get; set; }

    public static LopushokContext AppDbContext => new();
}
