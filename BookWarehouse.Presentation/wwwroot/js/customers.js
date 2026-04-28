var dataTable;
$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {

    dataTable = $('#customersTable').DataTable({
        "ajax": { url: '/DashBoard/CustomerDetailsData' },
        "processing": true,
        "language": {
            "processing": '<div class="spinner-border text-primary mt-3" role="status"><span class="visually-hidden">Loading...</span></div>'
        },
        "columns": [
            {
                data: 'firstName',
                "width": "35%",
                className: "align-middle ps-4",
                render: function (data, type, row) { 
                    let initials = (data ? data[0] : '') + (row.lastName ? row.lastName[0] : '');
                    return `
                        <div class="d-flex align-items-center gap-3">
                            <div class="rounded-circle d-flex justify-content-center align-items-center text-primary fw-bold shadow-sm" style="width: 40px; height: 40px; background-color: #e0f2fe; font-size: 0.9rem;">
                                ${initials.toUpperCase()}
                            </div>
                            <span class="fw-bold text-dark" style="font-size: 0.95rem;">${data} ${row.lastName}</span>
                        </div>
                    `;
                }
            },
            {
                data: 'email',
                "width": "45%",
                className: "align-middle",
                render: function (data) { return `<span class="text-muted fw-medium" style="font-size: 0.95rem;">${data}</span>`; }
            },
            {
                data: 'ordersCount',
                "width": "20%",
                className: "text-center align-middle",
                render: function (data) { 
                    return `
                        <div class="d-inline-flex flex-column justify-content-center align-items-center rounded-3" style="background-color: #f1f5f9; width: 48px; padding: 4px;">
                            <span class="fw-bold text-dark" style="font-size: 0.95rem;">${data}</span>
                            <span class="text-muted" style="font-size: 0.7rem;">orders</span>
                        </div>
                    `;
                }
            }
        ]
    });

}

