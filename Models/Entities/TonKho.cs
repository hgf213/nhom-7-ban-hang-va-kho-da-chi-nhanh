using System;
using System.Collections.Generic;

namespace QuanLyBanHangDaChiNhanh.Models.Entities;

public partial class TonKho
{
    public int MaCn { get; set; }

    public int MaSp { get; set; }

    public int SoLuongTon { get; set; }

    public int? MucTonToiThieu { get; set; }

    public virtual ChiNhanh MaCnNavigation { get; set; } = null!;

    public virtual SanPham MaSpNavigation { get; set; } = null!;
}
