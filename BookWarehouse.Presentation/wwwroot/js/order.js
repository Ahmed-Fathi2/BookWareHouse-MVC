var dataTable;
$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {

    dataTable = $('#orderTable').DataTable({
        "ajax": { url: '/Order/GetAll' },
        "columns": [
            { data: 'id', width: "15%", className: "text-center fw-bold" },

            { data: 'fullName', width: "17%", className: "text-start" },

            { data: 'phoneNumber', width: "13%", className: "text-start" },
            {
                data: 'orderDate',
                width: "18%",
                render: function (data) {
                    const date = new Date(data + "Z");

                    return date.toLocaleString('en-GB', {
                        timeZone: 'Africa/Cairo',
                        day: '2-digit',
                        month: '2-digit',
                        year: 'numeric',
                        hour: '2-digit',
                        minute: '2-digit',
                        hour12: true
                    });
                }
            },

            { data: 'orderStatus', width: "14%", className: "text-center" },

            { data: 'paymentStatus', width: "14%", className: "text-center" },
            {
           
                data: 'id',
                render: function (data) {
                    return `<div class="align-middle text-center" style="white-space: nowrap;">
                      
                            <a href="/order/Details/${data}" class="btn btn-sm btn-secondary m-1">
                                <i class="bi bi-info-circle"></i> Details
                            </a>
                         
                        </div>`;
                }
                , "width": "25%"
            }
        ]
    });

}

