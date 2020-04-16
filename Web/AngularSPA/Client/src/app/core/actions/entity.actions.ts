import { Action } from "@ngrx/store";
import { Predicate } from "@angular/core";
import { EntityAdapter, EntityState } from "@ngrx/entity";

export enum EntityActionTypes {
    LoadEntities = '[Entity] LoadEntities',
    AddEntity = '[Entity] AddEntity',
    AddEntities = '[Entity] AddEntities',
    UpdateEntity = '[Entity] UpdateEntity',
    UpsertEntity = '[Entity] UpsertEntity',
    UpsertEntities = '[Entity] UpsertEntities',
    DeleteEntity = '[Entity] DeleteEntity',
    DeleteEntities = '[Entity] DeleteEntities',
    DeleteEntitiesByPredicate = '[Entity] DeleteEntitiesByPredicate'
}

export class LoadEntities<T> implements Action {
    readonly type = EntityActionTypes.LoadEntities;

    constructor(public entityType: string, public payload: {entities: T[]}) {}
}

export class AddEntity<T> implements Action {
    readonly type = EntityActionTypes.AddEntity;

    constructor(public entityType: string, public payload: {entity: T}) {}
}

export class AddEntities<T> implements Action {
    readonly type = EntityActionTypes.AddEntities;

    constructor(public entityType: string, public payload: {entities: T[]}) {}
}

export class UpdateEntity<T> implements Action {
    readonly type = EntityActionTypes.UpdateEntity;

    constructor(public entityType: string, public payload: {entity: Partial<T>}) {}
}

export class UpsertEntity<T> implements Action {
    readonly type = EntityActionTypes.UpsertEntity;

    constructor(public entityType: string, public payload: {entity: T}) {}
}

export class UpsertEntities<T> implements Action {
    readonly type = EntityActionTypes.UpsertEntities;

    constructor(public entityType: string, public payload: {entities: T[]}) {}
}

export class DeleteEntity implements Action {
    readonly type = EntityActionTypes.DeleteEntity;

    constructor(public entityType: string, public payload: {id: string}) {}
}

export class DeleteEntities implements Action {
    readonly type = EntityActionTypes.DeleteEntities;

    constructor(public entityType: string, public payload: {ids: string[]}) {}
}

export class DeleteEntitiesByPredicate<T> implements Action {
    readonly type = EntityActionTypes.DeleteEntitiesByPredicate;

    constructor(public entityType: string, public payload: { predicate: Predicate<T>}) {}
}

export type EntityActionsUnion<T> =
    LoadEntities<T>
    | AddEntity<T>
    | AddEntities<T>
    | UpdateEntity<T>
    | UpsertEntity<T>
    | UpsertEntities<T>
    | DeleteEntity
    | DeleteEntities
    | DeleteEntitiesByPredicate<T>;

export function isEntityAction<T, U>(action: EntityActionsUnion<T> | U): action is EntityActionsUnion<T> {
    return (<EntityActionsUnion<T>>action).entityType !== undefined;
}