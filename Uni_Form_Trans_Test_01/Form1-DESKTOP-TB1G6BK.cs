using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TransParentModule;

namespace Uni_Form_Trans_Test_01
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      Bitmap bmp = (Bitmap)Properties.Resources.img_1;
      //SetBitmap(new Bitmap(bmp, new Size(100, 100)));
    }
  }
}
