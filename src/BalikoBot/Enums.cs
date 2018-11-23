using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace BalikoBot
{
	/// <summary>
	/// dopravci
	/// </summary>
	public enum Carriers
	{
		/// <summary>
		/// Česká pošta s.p.
		/// </summary>
		cp,
		/// <summary>
		/// DHL Express
		/// </summary>
		dhl,
		/// <summary>
		/// Direct Parcel Distribution CZ s.r.o.
		/// </summary>
		dpd,
		/// <summary>
		/// Geis CZ s.r.o., Parcel i Cargo, CZ i SK verze
		/// </summary>
		geis,
		/// <summary>
		/// General Logistics Systems Czech Republic s.r.o.
		/// </summary>
		gls,
		/// <summary>
		/// IN TIME SPEDICE s. r.o.
		/// </summary>
		intime,
		/// <summary>
		/// Pošta bez hranic (Frogman s.r.o.)
		/// </summary>
		pbh,
		/// <summary>
		/// PPL CZ s.r.o. – Parcel i Sprint
		/// </summary>
		ppl,
		/// <summary>
		/// Slovenská pošta a.s.
		/// </summary>
		sp,
		/// <summary>
		/// TOPTRANS EU a.s.
		/// </summary>
		toptrans,
		/// <summary>
		/// Uloženka s.r.o.
		/// </summary>
		ulozenka,
		/// <summary>
		/// Zásilkovna s.r.o.
		/// </summary>
		zasilkovna,
	}

	/// <summary>
	/// posledni stav baliku
	/// </summary>
	public enum BalikoBotTrackStatuses : int
	{
		NotInProcess = -1,
		Refused = 0,
		Delivered = 1,
		InDelivery = 2,
		Cancelled = 3,
		Returned = 4,
	}

	/// <summary>
	/// PSČ mohou být navrácena dvěma způsoby: buďto jako rozsahy (zip_range) nebo jednotlivě (zip),
	/// případně u zemí, kde PSČ nemají(například Spojené arabské emiráty) se vrací city.
	/// </summary>
	[DataContract]
	public enum ZipTypes : int
	{
		[DataMember(Name = "zip_range")]
		ZipRange,
		[DataMember(Name = "zip")]
		Zip,
		[DataMember(Name = "city")]
		City,
	}

	/// <summary>
	/// typ pobočky
	/// </summary>
	[DataContract]
	public enum BranchTypes : int
	{
		/// <summary>
		/// vydejni misto
		/// </summary>
		[DataMember(Name = "branch")]
		Branch,
		/// <summary>
		/// sluzba
		/// </summary>
		[DataMember(Name = "service")]
		Service,
	}
}
