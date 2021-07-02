import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'sort'
})
export class SortPipe  implements PipeTransform {
  transform(array: any, field: string, order: string = 'asc'): any[] {
    order = order.toLowerCase();
    if (order !== 'asc' && order !== 'desc') {
      return array;
    }
    if (!Array.isArray(array)) {
      return null as any;
    }
    array.sort((a: any, b: any) => {
      if (a[field] < b[field]) {
        return (order === 'asc') ? -1 : 1;
      } else if (a[field] > b[field]) {
        return (order === 'asc') ? 1 : -1;
      } else {
        return 0;
      }
    });
    return array;
  }
}
