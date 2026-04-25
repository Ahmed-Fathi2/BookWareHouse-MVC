var dataTable;
$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {

    dataTable = $('#producttbl').DataTable({
        "ajax": { url: '/AdminProduct/GetAll' },
        "processing": true,
        "language": {
            "processing": '<div class="spinner-border text-primary mt-3" role="status"><span class="visually-hidden">Loading...</span></div>'
        },
        "columns": [
            { 
                data: 'title', 
                "width": "25%",
                className: "align-middle ps-4",
                render: function(data) { return `<span class="fw-bold text-dark" style="font-size: 0.95rem;">${data}</span>`; }
            },
            { 
                data: 'isbn', 
                "width": "15%",
                className: "align-middle",
                render: function(data) { return `<span class="text-muted fw-medium" style="font-size: 0.9rem;">${data}</span>`; }
            },
            { 
                data: 'price', 
                "width": "10%", 
                className: "text-start align-middle",
                render: function(data) { return `<span class="fw-bolder text-dark" style="font-size: 0.95rem;">$${data.toFixed(2)}</span>`; }
            },
            { 
                data: 'author', 
                "width": "15%",
                className: "align-middle",
                render: function(data) { return `<span class="fw-medium text-dark" style="font-size: 0.9rem;">${data}</span>`; }
            },
            { 
                data: 'categoryName', 
                "width": "15%",
                className: "align-middle",
                render: function(data) { 
                    return `
                        <span class="badge rounded-pill bg-light text-secondary border fw-bold px-3 py-2" style="font-size: 0.8rem;">
                            ${data}
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
                            <a href="/AdminProduct/Edit/${data}" class="btn btn-sm btn-white border shadow-sm fw-bold rounded-3 px-3 d-flex align-items-center gap-1 text-dark hover-bg-light" style="font-size: 0.85rem;">
                                <i class="bi bi-pencil-square"></i> Edit
                            </a>
                            <a href="/AdminProduct/Details/${data}" class="btn btn-sm btn-white border shadow-sm rounded-3 px-2 text-muted hover-bg-light">
                                <i class="bi bi-info-circle"></i>
                            </a>
                            <a onClick=Delete('/AdminProduct/Delete/${data}') class="btn btn-sm btn-white border shadow-sm rounded-3 px-2 text-danger hover-bg-light">
                                <i class="bi bi-trash-fill"></i>
                            </a>
                        </div>
                    `;
                }, 
                "width": "20%"
            }
        ]
    });

}

function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.options =
                    {
                        closeButton: true,
                        progressBar: true,
                        timeOut: "2500",
                        showMethod: "slideDown",
                        hideMethod: "slideUp",
                    };
                    toastr.success(data.message);
                }
            })
        }
    })
}