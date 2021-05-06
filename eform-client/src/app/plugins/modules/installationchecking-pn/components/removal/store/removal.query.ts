import { Injectable } from '@angular/core';
import { Query } from '@datorama/akita';
import { RemovalState, RemovalStore } from './removal.store';
import { PaginationModel, SortModel } from 'src/app/common/models';

@Injectable({ providedIn: 'root' })
export class RemovalQuery extends Query<RemovalState> {
  constructor(protected store: RemovalStore) {
    super(store);
  }

  get pageSetting() {
    return this.getValue();
  }

  selectState$ = this.select((state) => state.filters.state);
  selectNameFilter$ = this.select((state) => state.filters.nameFilter);
  selectPageSize$ = this.select((state) => state.pagination.pageSize);
  selectPagination$ = this.select(
    (state) =>
      new PaginationModel(
        state.total,
        state.pagination.pageSize,
        state.pagination.offset
      )
  );
  selectSort$ = this.select(
    (state) => new SortModel(state.pagination.sort, state.pagination.isSortDsc)
  );
}
