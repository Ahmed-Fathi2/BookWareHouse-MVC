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
        $('#orderTable tbody').css('opacity', '0.1');
        
        var url = status === "all" ? '/Order/GetAll' : `/Order/GetAll?status=${status}`;
        
        // Smoothly reload data via Ajax without destroying the datatable
        dataTable.ajax.url(url).load(function() {
            // Restore opacity after data is drawn
            $('#orderTable tbody').css('opacity', '1');
        });
    });
});

function getStatusBadge(status) {
    let badgeClass = 'bg-secondary';
    switch (status) {
        case 'Approved': badgeClass = 'bg-success'; break;
        case 'Processing': badgeClass = 'bg-warning text-dark'; break;
        case 'Shipped': badgeClass = 'bg-info text-dark'; break;
        case 'Delivered': badgeClass = 'bg-success'; break;
        case 'Cancelled': badgeClass = 'bg-danger'; break;
        case 'Pending': badgeClass = 'bg-secondary'; break;
    }
    return `<span class="badge rounded-pill px-3 py-2 ${badgeClass} fw-bold shadow-sm" style="min-width: 90px; letter-spacing: 0.5px;">${status}</span>`;
}

function initDataTable(status) {
    var url = status === "all" ? '/Order/GetAll' : `/Order/GetAll?status=${status}`;

    dataTable = $('#orderTable').DataTable({
        "ajax": { url: url },
        "processing": true,
        "language": {
            "processing": '<div class="spinner-border text-primary mt-3" role="status"><span class="visually-hidden">Loading...</span></div>'
        },
        "columns": [
            { 
                data: 'id', width: "10%", className: "text-center align-middle", 
                render: function(data) { return `<span class="fw-bolder" style="color: #000; font-size: 0.95rem;">${data}</span>`; }
            },
            { 
                data: 'fullName', width: "15%", className: "text-start align-middle",
                render: function(data) { return `<span class="fw-bold" style="color: #000; font-size: 0.95rem;">${data}</span>`; }
            },
            { 
                data: 'phoneNumber', width: "16%", className: "text-start align-middle",
                render: function(data) { return `<span class="fw-bold" style="color: #000; font-size: 0.95rem;">${data}</span>`; }
            },
            {
                data: 'orderDate',
                width: "18%",
                className: "align-middle",
                render: function (data) {
                    const date = new Date(data + "Z");
                    let formatted = date.toLocaleString('en-GB', {
                        timeZone: 'Africa/Cairo',
                        day: '2-digit', month: 'short', year: 'numeric',
                        hour: '2-digit', minute: '2-digit', hour12: true
                    });
                    return `<span class="fw-bold" style="color: #000; font-size: 0.9rem;">${formatted}</span>`;
                }
            },
            { 
                data: 'orderStatus', 
                width: "13%", 
                className: "text-center align-middle",
                render: function(data) {
                    return getStatusBadge(data);
                }
            },
            { 
                data: 'paymentStatus', 
                width: "13%", 
                className: "text-center align-middle",
                render: function(data) {
                    let colorClass = "";
                    let iconClass = "";
                    if (data === "Approved" || data === "Paid" || data === "Completed") {
                        colorClass = "text-success";
                        iconClass = "bi-check2-circle";
                    } else if (data === "Rejected" || data === "Failed") {
                        colorClass = "text-danger";
                        iconClass = "bi-x-circle";
                    } else {
                        colorClass = "text-warning";  // Can also use a specific #color here if text-warning is too bright
                        iconClass = "bi-clock-history";
                    }
                    return `<span class="${colorClass} fw-bolder" style="font-size: 0.95rem;">
                                <i class="bi ${iconClass} me-1"></i>${data}
                            </span>`;
                }
            },
            {
                data: 'id',
                width: "13%",
                className: "text-center align-middle",
                render: function (data) {
                    return `<a href="/order/Details/${data}" class="btn btn-sm rounded-pill btn-outline-primary px-3 shadow-sm transition-all text-nowrap fw-bold" style="color: #0d6efd;">
                                <i class="bi bi-eye-fill me-1"></i> Details
                            </a>`;
                }
            }
        ],
        "drawCallback": function( settings ) {
            // Ensure opacity is restored on pagination/sort
            $('#orderTable tbody').css('opacity', '1');
        }
    });
}
