var dataTable;
$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {

  dataTable=  $('#producttbl').DataTable({
      "ajax": { url: '/AdminProduct/GetAll' },
        "columns": [
            { data: 'title', "width": "25%" },
            { data: 'isbn', "width": "25%" },
            { data: 'price', "width": "10%",className: "text-start" },
            { data: 'author', "width": "15%" },
            { data: 'categoryName',"width": "15%" },
            {
                data: 'id',
                render: function (data) {
                    return `<div class="align-middle text-center" style="white-space: nowrap;">
                            <a href="/AdminProduct/Edit/${data}" class="btn btn-sm btn-info m-1 text-white">
                                <i class="bi bi-pencil-square"></i> Edit
                            </a>
                            <a href="/AdminProduct/Details/${data}" class="btn btn-sm btn-secondary m-1">
                                <i class="bi bi-info-circle"></i> Details
                            </a>
                            <a onClick=Delete('/AdminProduct/Delete/${data}') class="btn btn-sm btn-danger m-1">
                                <i class="bi bi-trash-fill"></i> Delete
                            </a>
                        </div>`;
                }
                , "width": "25%"
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