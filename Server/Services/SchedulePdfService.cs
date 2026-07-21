using GIBS.Module.Resource.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GIBS.Module.Resource.Services
{
    public interface ISchedulePdfService
    {
        byte[] GenerateDailySchedulePdf(DateTime date, string resourceName, List<Reservation> reservations, Dictionary<int, string> resourceNames = null);
        byte[] GenerateWeeklySchedulePdf(DateTime weekStartDate, string resourceName, List<Reservation> reservations);
    }

    public class SchedulePdfService : ISchedulePdfService
    {
        static SchedulePdfService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GenerateDailySchedulePdf(DateTime date, string resourceName, List<Reservation> reservations, Dictionary<int, string> resourceNames = null)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    page.Margin(1, Unit.Inch);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(container => ComposeDailyContent(container, date, resourceName, reservations, resourceNames));
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }

        public byte[] GenerateWeeklySchedulePdf(DateTime weekStartDate, string resourceName, List<Reservation> reservations)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter.Landscape());
                    page.Margin(0.75f, Unit.Inch);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(container => ComposeWeeklyContent(container, weekStartDate, resourceName, reservations));
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }

        private void ComposeHeader(IContainer container)
        {
            container.Column(column =>
            {
                column.Spacing(5);

                column.Item().AlignCenter().Text("Resource Schedule")
                    .FontSize(18)
                    .Bold()
                    .FontColor(Colors.Blue.Darken3);

                column.Item().Height(2).Background(Colors.Blue.Lighten2);
            });
        }

        private void ComposeDailyContent(IContainer container, DateTime date, string resourceName, List<Reservation> reservations, Dictionary<int, string> resourceNames = null)
        {
            container.Column(column =>
            {
                column.Spacing(10);

                // Date and Resource Header
                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text($"Date: {date:dddd, MMMM dd, yyyy}").FontSize(14).Bold();
                        col.Item().Text($"Resource: {resourceName}").FontSize(12).FontColor(Colors.Grey.Darken2);
                    });
                    row.ConstantItem(150).AlignRight().Text($"Generated: {DateTime.Now:g}").FontSize(8).FontColor(Colors.Grey.Medium);
                });

                // Reservations Table
                if (reservations.Any())
                {
                    var groupedReservations = resourceName == "All Resources" 
                        ? reservations.GroupBy(r => r.ResourceId).OrderBy(g => resourceNames != null && resourceNames.ContainsKey(g.Key) ? resourceNames[g.Key] : "Unknown")
                        : new[] { reservations.GroupBy(r => r.ResourceId).First() }.AsEnumerable();

                    foreach (var group in groupedReservations)
                    {
                        if (resourceName == "All Resources")
                        {
                            var groupResourceName = resourceNames != null && resourceNames.ContainsKey(group.Key) 
                                ? resourceNames[group.Key] 
                                : $"Resource ID {group.Key}";
                            column.Item().PaddingTop(10).Text($"Resource: {groupResourceName}").FontSize(12).Bold();
                        }

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3); // Time
                                columns.RelativeColumn(3); // User
                                columns.RelativeColumn(2); // Status
                                columns.RelativeColumn(5); // Notes
                            });

                            // Header
                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Background(Colors.Blue.Lighten3).Text("Time").Bold();
                                header.Cell().Element(CellStyle).Background(Colors.Blue.Lighten3).Text("User").Bold();
                                header.Cell().Element(CellStyle).Background(Colors.Blue.Lighten3).Text("Status").Bold();
                                header.Cell().Element(CellStyle).Background(Colors.Blue.Lighten3).Text("Notes").Bold();
                            });

                            // Rows
                            foreach (var reservation in group.OrderBy(r => r.StartTime))
                            {
                                table.Cell().Element(CellStyle).Text($"{reservation.StartTime:h:mm tt} - {reservation.EndTime:h:mm tt}");
                                table.Cell().Element(CellStyle).Text(reservation.UserName ?? "Unknown");
                                table.Cell().Element(CellStyle).Text(reservation.Status.ToString())
                                    .FontColor(GetStatusColor(reservation.Status));
                                table.Cell().Element(CellStyle).Text(string.IsNullOrWhiteSpace(reservation.Notes) ? "-" : reservation.Notes);
                            }
                        });
                    }
                }
                else
                {
                    column.Item().PaddingTop(20).AlignCenter()
                        .Border(1)
                        .BorderColor(Colors.Blue.Lighten2)
                        .Background(Colors.Blue.Lighten4)
                        .Padding(20)
                        .Text("No bookings found for this date")
                        .FontSize(12)
                        .FontColor(Colors.Grey.Darken1);
                }
            });
        }

        private void ComposeWeeklyContent(IContainer container, DateTime weekStartDate, string resourceName, List<Reservation> reservations)
        {
            container.Column(column =>
            {
                column.Spacing(10);

                // Week and Resource Header
                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text($"Week of {weekStartDate:MMMM dd, yyyy} - {weekStartDate.AddDays(6):MMMM dd, yyyy}").FontSize(14).Bold();
                        col.Item().Text($"Resource: {resourceName}").FontSize(12).FontColor(Colors.Grey.Darken2);
                    });
                    row.ConstantItem(150).AlignRight().Text($"Generated: {DateTime.Now:g}").FontSize(8).FontColor(Colors.Grey.Medium);
                });

                // Weekly Grid
                if (reservations.Any())
                {
                    var dayGroups = reservations.GroupBy(r => r.StartTime.Date).OrderBy(g => g.Key).ToList();

                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2); // Day
                            columns.RelativeColumn(2); // Time
                            columns.RelativeColumn(3); // User
                            columns.RelativeColumn(2); // Status
                            columns.RelativeColumn(4); // Notes
                        });

                        // Header
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Background(Colors.Blue.Lighten3).Text("Day").Bold();
                            header.Cell().Element(CellStyle).Background(Colors.Blue.Lighten3).Text("Time").Bold();
                            header.Cell().Element(CellStyle).Background(Colors.Blue.Lighten3).Text("User").Bold();
                            header.Cell().Element(CellStyle).Background(Colors.Blue.Lighten3).Text("Status").Bold();
                            header.Cell().Element(CellStyle).Background(Colors.Blue.Lighten3).Text("Notes").Bold();
                        });

                        foreach (var dayGroup in dayGroups)
                        {
                            bool isFirstRow = true;
                            foreach (var reservation in dayGroup.OrderBy(r => r.StartTime))
                            {
                                if (isFirstRow)
                                {
                                    table.Cell().RowSpan((uint)dayGroup.Count()).Element(CellStyle)
                                        .Background(Colors.Grey.Lighten3)
                                        .Text($"{reservation.StartTime:ddd}\n{reservation.StartTime:MMM dd}")
                                        .Bold();
                                    isFirstRow = false;
                                }

                                table.Cell().Element(CellStyle).Text($"{reservation.StartTime:h:mm tt} - {reservation.EndTime:h:mm tt}");
                                table.Cell().Element(CellStyle).Text(reservation.UserName ?? "Unknown");
                                table.Cell().Element(CellStyle).Text(reservation.Status.ToString())
                                    .FontColor(GetStatusColor(reservation.Status));
                                table.Cell().Element(CellStyle).Text(string.IsNullOrWhiteSpace(reservation.Notes) ? "-" : reservation.Notes);
                            }
                        }
                    });
                }
                else
                {
                    column.Item().PaddingTop(20).AlignCenter()
                        .Border(1)
                        .BorderColor(Colors.Blue.Lighten2)
                        .Background(Colors.Blue.Lighten4)
                        .Padding(20)
                        .Text("No bookings found for this week")
                        .FontSize(12)
                        .FontColor(Colors.Grey.Darken1);
                }
            });
        }

        private IContainer CellStyle(IContainer container)
        {
            return container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5);
        }

        private string GetStatusColor(ReservationStatus status)
        {
            return status switch
            {
                ReservationStatus.Confirmed => Colors.Green.Darken2,
                ReservationStatus.Pending => Colors.Orange.Darken1,
                ReservationStatus.Cancelled => Colors.Grey.Medium,
                _ => Colors.Black
            };
        }
    }
}
