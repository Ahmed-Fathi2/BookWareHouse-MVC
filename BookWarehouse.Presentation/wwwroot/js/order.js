var dataTable;

$(document).ready(function () {
    // Load initially with "all" status
    initDataTable("all");

    // Listen to clicks on the new dropdown filter options
    $('.filter-dropdown-btn').on('click', function (e) {
        e.preventDefault();
        
        // Handle UI update
        $('.filter-dropdown-btn').removeClass('active');
        $(this).addClass('active');
        
        // Update the button text
        var text = $(this).text().trim();
        $('#currentFilterText').text(text);
        
        var status = $(this).data('status');
        
        // Add smooth opacity fade out
        $('#orderTable tbody').css('opacity', '0.3');
        
        var url = status === "all" ? '/Order/GetAll' : `/Order/GetAll?status=${status}`;
        
        // Smoothly reload data via Ajax without destroying the datatable
        dataTable.ajax.url(url).load(function() {
            // Restore opacity after data is drawn
            $('#orderTable tbody').css('opacity', '1');
        });
    });
});

function initDataTable(status) {
    var url = status === "all" ? '/Order/GetAll' : `/Order/GetAll?status=${status}`;

    dataTable = $('#orderTable').DataTable({
        "ajax": { url: url },
        "processing": true,
        "language": {
            "processing": '<div class="spinner-border text-primary mt-3" role="status"><span class="visually-hidden">Loading...</span></div>'
        },
        // We do not want table borders managed by datatables css, so we keep classes simple in HTML
        "columns": [
            { 
                data: 'id', 
                className: "align-middle", 
                render: function(data) { return `<span class="fw-bolder text-dark" style="font-size: 0.95rem;">#${data}</span>`; }
            },
            { 
                data: 'fullName', 
                className: "align-middle",
                render: function(data, type, row) { 
                    let initials = data.split(' ').map(n => n[0]).join('').substring(0,2).toUpperCase();
                    let email = row.email || (data.split(' ')[0].toLowerCase() + "@mail.com"); // fallback if email is null
                    return `
                        <div class="d-flex align-items-center gap-3">
                            <div class="rounded-circle d-flex justify-content-center align-items-center text-primary fw-bold" style="width: 42px; height: 42px; background-color: #e0f2fe; font-size: 0.95rem;">
                                ${initials}
                            </div>
                            <div class="d-flex flex-column">
                                <span class="fw-bold text-dark" style="font-size: 0.95rem;">${data}</span>
                                <span class="text-muted" style="font-size: 0.85rem;">${email}</span>
                            </div>
                        </div>
                    `; 
                }
            },
            { 
                data: 'phoneNumber', 
                className: "align-middle text-nowrap",
                render: function(data) { return `<span class="text-muted fw-medium" style="font-size: 0.95rem;">${data}</span>`; }
            },
            {
                data: 'orderDate',
                className: "align-middle",
                render: function (data) {
                    const date = new Date(data + "Z");
                    let dateStr = date.toLocaleString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' });
                    let timeStr = date.toLocaleString('en-GB', { hour: '2-digit', minute: '2-digit', hour12: true }).toLowerCase();
                    return `
                        <div class="d-flex flex-column">
                            <span class="text-dark fw-medium" style="font-size: 0.95rem;">${dateStr},</span>
                            <span class="text-muted" style="font-size: 0.9rem;">${timeStr}</span>
                        </div>
                    `;
                }
            },
            { 
                data: 'items', 
                className: "align-middle",
                render: function(data) {
                    let count = data ? data.length : 0;
                    return `
                        <div class="d-flex flex-column justify-content-center align-items-center rounded-3" style="background-color: #f1f5f9; width: 48px; padding: 4px;">
                            <span class="fw-bold text-dark" style="font-size: 0.95rem;">${count}</span>
                            <span class="text-muted" style="font-size: 0.75rem;">items</span>
                        </div>
                    `;
                }
            },
            { 
                data: 'orderTotal', 
                className: "align-middle",
                render: function(data) { return `<span class="fw-bolder text-dark" style="font-size: 0.95rem;">$${data.toFixed(2)}</span>`; }
            },
            { 
                data: 'orderStatus', 
                className: "text-center align-middle",
                render: function(data) {
                    let bgClass = "bg-light";
                    let textClass = "text-secondary";
                    let dotColor = "#6c757d";
                    
                    if (data === "Delivered" || data === "Approved") { bgClass = "bg-success bg-opacity-10"; textClass = "text-success"; dotColor = "#198754"; }
                    else if (data === "Pending") { bgClass = "bg-warning bg-opacity-10"; textClass = "text-warning"; dotColor = "#ffc107"; }
                    else if (data === "Processing") { bgClass = "bg-info bg-opacity-10"; textClass = "text-info"; dotColor = "#0dcaf0"; }
                    else if (data === "Shipped") { bgClass = "bg-primary bg-opacity-10"; textClass = "text-primary"; dotColor = "#0d6efd"; }
                    else if (data === "Cancelled") { bgClass = "bg-danger bg-opacity-10"; textClass = "text-danger"; dotColor = "#dc3545"; }

                    return `
                        <span class="badge rounded-pill px-3 py-2 ${bgClass} ${textClass} fw-bold" style="font-size: 0.85rem;">
                            <span class="rounded-circle d-inline-block me-2" style="width: 6px; height: 6px; background-color: ${dotColor}; vertical-align: middle; margin-top: -1px;"></span>${data}
                        </span>
                    `;
                }
            },
            { 
                data: 'paymentStatus', 
                className: "text-center align-middle",
                render: function(data) {
                    let bgClass = "bg-light";
                    let textClass = "text-secondary";
                    
                    if (data === "Paid" || data === "Approved") { bgClass = "bg-success bg-opacity-10"; textClass = "text-success"; data = "Paid"; }
                    else if (data === "Failed" || data === "Rejected") { bgClass = "bg-danger bg-opacity-10"; textClass = "text-danger"; data = "Failed"; }
                    else { bgClass = "bg-warning bg-opacity-10"; textClass = "text-warning"; data = "Pending"; }

                    return `
                        <span class="badge rounded-pill px-3 py-2 ${bgClass} ${textClass} fw-bold d-inline-flex align-items-center gap-2" style="font-size: 0.85rem; border: 1px solid currentColor;">
                            <i class="bi bi-credit-card"></i> ${data}
                        </span>
                    `;
                }
            },
            {
                data: 'id',
                className: "text-center align-middle",
                render: function (data) {
                    return `
                        <div class="d-flex justify-content-center align-items-center gap-2">
                            <a href="/Order/Details/${data}" class="btn btn-sm btn-white border shadow-sm fw-bold rounded-3 px-3 d-flex align-items-center gap-1 text-dark hover-bg-light" style="font-size: 0.85rem;">
                                <i class="bi bi-eye"></i> View
                            </a>
                        </div>
                    `;
                }
            }
        ],
        "drawCallback": function( settings ) {
            // Ensure opacity is restored on pagination/sort
            $('#orderTable tbody').css('opacity', '1');
        }
    });
}
