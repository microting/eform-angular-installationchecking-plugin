import { Injectable } from '@angular/core';
import { persistState, Store, StoreConfig } from '@datorama/akita';
import { CommonPaginationState } from 'src/app/common/models/common-pagination-state';
import { FiltrationStateModel } from 'src/app/common/models';
import {
  InstallationStateEnum,
  InstallationTypeEnum,
} from 'src/app/plugins/modules/installationchecking-pn/const';

export interface InstallationState {
  pagination: CommonPaginationState;
  filters: InstallationFiltrationState;
  total: number;
}

export class InstallationFiltrationState extends FiltrationStateModel {
  type: InstallationTypeEnum;
  state: InstallationStateEnum;
}

function createInitialState(): InstallationState {
  return <InstallationState>{
    pagination: {
      pageSize: 10,
      sort: 'Id',
      isSortDsc: false,
      offset: 0,
    },
    filters: {
      nameFilter: '',
      type: InstallationTypeEnum.Installation,
      state: 0,
    },
    total: 0,
  };
}

const installationPersistStorage = persistState({
  include: ['installation'],
  key: 'installationCheckingPn',
  preStorageUpdate(storeName, state) {
    return {
      pagination: state.pagination,
      filters: state.filters,
    };
  },
});

@Injectable({ providedIn: 'root' })
@StoreConfig({ name: 'installation', resettable: true })
export class InstallationStore extends Store<InstallationState> {
  constructor() {
    super(createInitialState());
  }
}

export const installationPersistProvider = {
  provide: 'persistStorage',
  useValue: installationPersistStorage,
  multi: true,
};
